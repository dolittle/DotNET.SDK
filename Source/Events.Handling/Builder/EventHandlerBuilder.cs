// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Common.Model;
using Dolittle.SDK.Events.Handling.Builder.Methods;

namespace Dolittle.SDK.Events.Handling.Builder;

/// <summary>
/// Represents a building event handlers.
/// </summary>
public class EventHandlerBuilder : IEventHandlerBuilder, ICanTryBuildEventHandler, IEquatable<EventHandlerBuilder>
{
    readonly EventHandlerId _eventHandlerId;
    readonly IModelBuilder _modelBuilder;
    readonly EventHandlerMethodsBuilder _methodsBuilder;

    bool _partitioned = true;
    int _concurrency = 1;
    ScopeId _scopeId = ScopeId.Default;
    EventHandlerAlias? _alias;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventHandlerBuilder"/> class.
    /// </summary>
    /// <param name="eventHandlerId">The <see cref="EventHandlerId" />.</param>
    /// <param name="modelBuilder">The <see cref="IModelBuilder"/>.</param>
    public EventHandlerBuilder(EventHandlerId eventHandlerId, IModelBuilder modelBuilder)
    {
        _eventHandlerId = eventHandlerId;
        _modelBuilder = modelBuilder;
        _methodsBuilder = new EventHandlerMethodsBuilder(_eventHandlerId);
        Bind();
    }

    /// <summary>
    /// Gets the <see cref="EventHandlerModelId"/>.
    /// </summary>
    public EventHandlerModelId ModelId => new(_eventHandlerId, _partitioned, _scopeId, alias: _alias?.Value, concurrency: _concurrency);

    /// <inheritdoc />
    public IEventHandlerMethodsBuilder Partitioned()
    {
        Unbind();
        _partitioned = true;
        Bind();
        return _methodsBuilder;
    }

    /// <inheritdoc />
    public IEventHandlerMethodsBuilder Unpartitioned()
    {
        Unbind();
        _partitioned = false;
        Bind();
        return _methodsBuilder;
    }

    /// <inheritdoc />
    public IEventHandlerBuilder WithConcurrency(int concurrency)
    {
        Unbind();
        _concurrency = concurrency > 0 ? concurrency : 1;
        Bind();
        return this;
    }

    
    /// <inheritdoc />
    public IEventHandlerBuilder InScope(ScopeId scopeId)
    {
        Unbind();
        _scopeId = scopeId;
        Bind();
        return this;
    }

    /// <inheritdoc />
    public IEventHandlerBuilder WithAlias(EventHandlerAlias alias)
    {
        Unbind();
        _alias = alias;
        Bind();
        return this;
    }

    /// <inheritdoc />
    public bool TryBuild(EventHandlerModelId identifier, IEventTypes eventTypes, IClientBuildResults buildResults, out IEventHandler eventHandler)
    {
        eventHandler = default;
        var eventTypesToMethods = new Dictionary<EventType, IEventHandlerMethod>();
        if (!_methodsBuilder.TryAddEventHandlerMethods(identifier, eventTypes, eventTypesToMethods, buildResults))
        {
            buildResults.AddFailure(identifier, "One or more event handler methods could not be built");
            return false;
        }

        if (eventTypesToMethods.Count < 1)
        {
            buildResults.AddFailure(identifier, "No event types to handle");
            return false;
        }

        eventHandler = new EventHandler(identifier, eventTypesToMethods);
        return true;
    }

    /// <inheritdoc />
    public bool Equals(EventHandlerBuilder other)
        => ReferenceEquals(this, other);

    /// <inheritdoc />
    public override bool Equals(object other)
        => Equals(other as EventHandlerBuilder);

    /// <inheritdoc />
    public override int GetHashCode() =>
        HashCode.Combine(_eventHandlerId, _methodsBuilder, _alias, _partitioned, _scopeId);

    void Bind()
    {
        _modelBuilder.BindIdentifierToProcessorBuilder(ModelId, this);
    }

    void Unbind()
    {
        _modelBuilder.UnbindIdentifierToProcessorBuilder(ModelId, this);
    }
}

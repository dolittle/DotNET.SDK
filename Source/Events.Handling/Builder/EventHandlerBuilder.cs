// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.SDK.ApplicationModel.ClientSetup;
using Dolittle.SDK.ApplicationModel;
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

    EventHandlerAlias _alias;
    bool _hasAlias;
    bool _partitioned = true;
    ScopeId _scopeId = ScopeId.Default;

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
    public EventHandlerModelId ModelId => new(_eventHandlerId, _scopeId);
    
    /// <inheritdoc />
    public IEventHandlerMethodsBuilder Partitioned()
    {
        _partitioned = true;
        return _methodsBuilder;
    }

    /// <inheritdoc />
    public IEventHandlerMethodsBuilder Unpartitioned()
    {
        _partitioned = false;
        return _methodsBuilder;
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
        _alias = alias;
        _hasAlias = true;
        return this;
    }

    /// <inheritdoc />
    public bool TryBuild(IEventTypes eventTypes, IClientBuildResults buildResults, out IEventHandler eventHandler)
    {
        eventHandler = default;
        var eventTypesToMethods = new Dictionary<EventType, IEventHandlerMethod>();
        if (!_methodsBuilder.TryAddEventHandlerMethods(eventTypes, eventTypesToMethods, buildResults))
        {
            buildResults.AddFailure($"Failed to build event handler {_eventHandlerId}. One or more event handler methods could not be built");
            return false;
        }

        if (eventTypesToMethods.Count < 1)
        {
            buildResults.AddFailure($"Failed to build event handler {_eventHandlerId}. No event handler methods are configured for event handler");
            return false;
        }

        eventHandler = _hasAlias
            ? new EventHandler(_eventHandlerId, _alias, _scopeId, _partitioned, eventTypesToMethods)
            : new EventHandler(_eventHandlerId, _scopeId, _partitioned, eventTypesToMethods);
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
        HashCode.Combine(_eventHandlerId, _methodsBuilder, _alias, _hasAlias, _partitioned, _scopeId);

    void Bind()
    {
        _modelBuilder.BindIdentifierToProcessorBuilder(ModelId, this);
    }
    void Unbind()
    {
        _modelBuilder.UnbindIdentifierToProcessorBuilder(ModelId, this);
    }
}

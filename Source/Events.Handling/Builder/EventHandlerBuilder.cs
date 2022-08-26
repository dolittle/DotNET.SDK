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
public class EventHandlerBuilder : IEventHandlerBuilder, ICanTryBuildEventHandler
{
    readonly EventHandlerId _eventHandlerId;
    readonly IModelBuilder _modelBuilder;
    readonly EventHandlerMethodsBuilder _methodsBuilder;

    IdentifierAlias? _alias;
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

    EventHandlerModelId ModelId => new(_eventHandlerId, _partitioned, _scopeId, _alias);
    
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
    public IEventHandlerBuilder InScope(ScopeId scopeId)
    {
        Unbind();
        _scopeId = scopeId;
        Bind();
        return this;
    }

    /// <inheritdoc />
    public IEventHandlerBuilder WithAlias(IdentifierAlias alias)
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

        eventHandler = new EventHandler(identifier, eventTypesToMethods);
        return true;
    }
    
    /// <inheritdoc />
    public bool Equals(IProcessorBuilder<EventHandlerModelId, EventHandlerId> other)
        => other is EventHandlerBuilder && ReferenceEquals(this, other);

    /// <inheritdoc />
    public override bool Equals(object other)
        => Equals(other as EventHandlerBuilder);

    /// <inheritdoc />
    public override int GetHashCode() =>
        HashCode.Combine(_eventHandlerId, _methodsBuilder, _alias, _partitioned, _scopeId);

    void Bind()
    {
        _modelBuilder.BindIdentifierToProcessorBuilder<EventHandlerBuilder, EventHandlerModelId, EventHandlerId>(ModelId, this);
    }
    void Unbind()
    {
        _modelBuilder.UnbindIdentifierToProcessorBuilder<EventHandlerBuilder, EventHandlerModelId, EventHandlerId>(ModelId, this);
    }
}

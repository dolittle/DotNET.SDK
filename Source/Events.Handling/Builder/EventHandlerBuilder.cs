// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.DependencyInversion;
using Dolittle.SDK.Events.Handling.Builder.Methods;

namespace Dolittle.SDK.Events.Handling.Builder;

/// <summary>
/// Represents a building event handlers.
/// </summary>
public class EventHandlerBuilder : IEventHandlerBuilder, ICanTryBuildEventHandler
{
    readonly EventHandlerId _eventHandlerId;
    readonly EventHandlerMethodsBuilder _methodsBuilder;

    ScopeId _scopeId = ScopeId.Default;

    EventHandlerAlias _alias;
    bool _hasAlias;
    bool _partitioned = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventHandlerBuilder"/> class.
    /// </summary>
    /// <param name="eventHandlerId">The <see cref="EventHandlerId" />.</param>
    public EventHandlerBuilder(EventHandlerId eventHandlerId)
    {
        _eventHandlerId = eventHandlerId;
        _methodsBuilder = new EventHandlerMethodsBuilder(_eventHandlerId);
    }
    
    /// <inheritdoc />
    public bool TryGetIdentifier(out EventHandlerId identifier)
    {
        identifier = _eventHandlerId;
        return true;
    }

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
        _scopeId = scopeId;
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
    public bool TryBuild(IEventTypes eventTypes, IClientBuildResults buildResults, Func<ITenantScopedProviders> tenantScopedProvidersFactory, out IEventHandler eventHandler)
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
}

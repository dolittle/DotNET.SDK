// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.SDK.Common;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.DependencyInversion;

namespace Dolittle.SDK.Events.Handling.Builder.Convention.Instance;

/// <summary>
/// Represents an implementation of <see cref="IConventionInstanceEventHandlers"/>.
/// </summary>
public class ConventionInstanceEventHandlers : UniqueBindings<EventHandlerId, object>, IConventionInstanceEventHandlers
{
    /// <summary>
    /// Initializes an instance of the <see cref="ConventionInstanceEventHandlers"/> class. 
    /// </summary>
    /// <param name="bindings">The convention instance event handler bindings.</param>
    public ConventionInstanceEventHandlers(IUniqueBindings<EventHandlerId, object> bindings)
        : base(bindings)
    {
    }

    /// <inheritdoc />
    public IUniqueBindings<EventHandlerId, IEventHandler> Build(
        IEventTypes eventTypes,
        IClientBuildResults buildResults,
        System.Func<ITenantScopedProviders> tenantScopedProvidersFactory)
    {
        var eventHandlers = new Dictionary<EventHandlerId, IEventHandler>();
        foreach (var (eventHandlerId, instance) in Bindings)
        {
            if (new ConventionInstanceEventHandlerBuilder(instance).TryBuild(eventTypes, buildResults, tenantScopedProvidersFactory, out var eventHandler))
            {
                eventHandlers.Add(eventHandlerId, eventHandler);
            }
        }

        return new UniqueBindings<EventHandlerId, IEventHandler>(eventHandlers);
    }
}

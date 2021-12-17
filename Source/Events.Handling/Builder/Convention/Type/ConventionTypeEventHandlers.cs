// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.SDK.Common;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.DependencyInversion;

namespace Dolittle.SDK.Events.Handling.Builder.Convention.Type;

/// <summary>
/// Represents an implementation of <see cref="IConventionTypeEventHandlers"/>.
/// </summary>
public class ConventionTypeEventHandlers : UniqueBindings<EventHandlerId, System.Type>, IConventionTypeEventHandlers
{
    /// <summary>
    /// Initializes an instance of the <see cref="ConventionTypeEventHandlers"/> class.
    /// </summary>
    /// <param name="bindings"></param>
    public ConventionTypeEventHandlers(IUniqueBindings<EventHandlerId, System.Type> bindings)
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
        foreach (var (eventHandlerId, type) in Bindings)
        {
            if (new ConventionTypeEventHandlerBuilder(type).TryBuild(eventTypes, buildResults, tenantScopedProvidersFactory, out var eventHandler))
            {
                eventHandlers.Add(eventHandlerId, eventHandler);
            }
        }

        return new UniqueBindings<EventHandlerId, IEventHandler>(eventHandlers);
    }
}

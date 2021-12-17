// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dolittle.SDK.Common;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.DependencyInversion;
using Dolittle.SDK.Events.Handling.Builder.Convention.Instance;
using Dolittle.SDK.Events.Handling.Builder.Convention.Type;

namespace Dolittle.SDK.Events.Handling.Builder;

/// <summary>
/// Represents an implementation of <see cref="IEventHandlersBuilder"/>.
/// </summary>
public class EventHandlersBuilder : ClientUniqueBindingsBuilder<EventHandlerId, IEventHandler, IEventHandlerBindings>, IEventHandlersBuilder
{
    readonly ConventionTypeEventHandlerBindingsBuilder _conventionTypeEventHandlers = new();
    readonly ConventionInstanceEventHandlerBindingsBuilder _conventionInstanceEventHandlers = new();
    readonly EventHandlerBuilderBindingsBuilder _evenHandlerBuilders = new();

    IConventionTypeEventHandlers _builtConventionTypeEventHandlers;
    IConventionInstanceEventHandlers _builtConventionInstanceEventHandlers;

    /// <inheritdoc />
    public IEventHandlerBuilder CreateEventHandler(EventHandlerId eventHandlerId)
    {
        var builder = new EventHandlerBuilder(eventHandlerId);
        _evenHandlerBuilders.Add(eventHandlerId, builder);
        return builder;
    }

    /// <inheritdoc />
    public IEventHandlersBuilder RegisterEventHandler<TEventHandler>()
        where TEventHandler : class
        => RegisterEventHandler(typeof(TEventHandler));

    /// <inheritdoc />
    public IEventHandlersBuilder RegisterEventHandler(Type type)
    {
        _conventionTypeEventHandlers.Add(type);
        return this;
    }

    /// <inheritdoc />
    public IEventHandlersBuilder RegisterEventHandler<TEventHandler>(TEventHandler eventHandlerInstance)
        where TEventHandler : class
    {
        _conventionInstanceEventHandlers.Add(eventHandlerInstance);
        return this;
    }

    /// <inheritdoc />
    public IEventHandlersBuilder RegisterAllFrom(Assembly assembly)
    {
        foreach (var type in assembly.ExportedTypes)
        {
            if (IsEventHandler(type))
            {
                RegisterEventHandler(type);
            }
        }

        return this;
    }

    /// <summary>
    /// Build all event handlers.
    /// </summary>
    /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
    /// <param name="tenantScopedProvidersFactory">The <see cref="Func{TResult}"/> for getting <see cref="ITenantScopedProviders" />.</param>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    public IUnregisteredEventHandlers Build(IEventTypes eventTypes, Func<ITenantScopedProviders> tenantScopedProvidersFactory, IClientBuildResults buildResults)
    {
        foreach (var eventHandler in BuildAllEventHandlers(eventTypes, tenantScopedProvidersFactory, buildResults))
        {
            Add(eventHandler.Identifier, eventHandler);
        }

        return new UnregisteredEventHandlers(base.Build(buildResults));
    }

    /// <inheritdoc />
    protected override IEventHandlerBindings CreateUniqueBindings(IClientBuildResults aggregatedBuildResults, IUniqueBindings<EventHandlerId, IEventHandler> bindings)
        => new EventHandlerBindings(_builtConventionTypeEventHandlers, _builtConventionInstanceEventHandlers, bindings);

    static bool IsEventHandler(Type type)
        => type.GetCustomAttributes(typeof(EventHandlerAttribute), true).FirstOrDefault() is EventHandlerAttribute;
    
    IEnumerable<IEventHandler> BuildAllEventHandlers(
        IEventTypes eventTypes,
        Func<ITenantScopedProviders> tenantScopedProvidersFactory,
        IClientBuildResults buildResults)
    {
        _builtConventionTypeEventHandlers = _conventionTypeEventHandlers.Build(buildResults);
        _builtConventionInstanceEventHandlers = _conventionInstanceEventHandlers.Build(buildResults);
        var builtEventHandlerBuilders = _evenHandlerBuilders.Build(buildResults);
        var eventHandlerBuilders = new ICanBuildEventHandlerBindings[]
        {
            _builtConventionInstanceEventHandlers,
            _builtConventionTypeEventHandlers,
            builtEventHandlerBuilders
        };
        return eventHandlerBuilders
            .Select(_ => _.Build(eventTypes, buildResults, tenantScopedProvidersFactory))
            .SelectMany(_ => _.Values);
    }
}

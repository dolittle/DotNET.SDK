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
public class EventHandlersBuilder : IEventHandlersBuilder
{
    readonly ClientUniqueBindingsBuilder<EventHandlerId, IEventHandler> _eventHandlerBindingsBuilder = new(valueLabel: "Event Handler");
    readonly ClientUniqueDecoratedBindingsBuilder<EventHandlerId, Type, EventHandlerAttribute> _conventionTypeBindingsBuilder = new(valueLabel: "Event Handler Convention Type");
    readonly ClientUniqueDecoratedBindingsBuilder<EventHandlerId, object, EventHandlerAttribute> _conventionInstanceBindingsBuilder = new(valueLabel: "Event Handler Convention Instance");
    readonly ClientUniqueBindingsBuilder<EventHandlerId, EventHandlerBuilder> _builderBindingsBuilder = new(valueLabel: "Event Handler Builder");
    
    IConventionInstanceEventHandlers _builtConventionInstanceEventHandlers;

    readonly List<ConventionInstanceEventHandlerBuilder> _conventionInstanceBuilders = new();
    readonly List<ConventionTypeEventHandlerBuilder> _conventionTypeBuilders = new();
    readonly List<EventHandlerBuilder> _builders = new();

    /// <inheritdoc />
    public IEventHandlerBuilder CreateEventHandler(EventHandlerId eventHandlerId)
    {
        var builder = new EventHandlerBuilder(eventHandlerId);
        _builders.Add(builder);
        _builderBindingsBuilder.Add(eventHandlerId, builder);
        return builder;
    }

    /// <inheritdoc />
    public IEventHandlersBuilder RegisterEventHandler<TEventHandler>()
        where TEventHandler : class
        => RegisterEventHandler(typeof(TEventHandler));

    /// <inheritdoc />
    public IEventHandlersBuilder RegisterEventHandler(Type type)
    {
        _conventionTypeBuilders.Add(new ConventionTypeEventHandlerBuilder(type));
        _conventionTypeBindingsBuilder.Add(type);
        return this;
    }

    /// <inheritdoc />
    public IEventHandlersBuilder RegisterEventHandler<TEventHandler>(TEventHandler eventHandlerInstance)
        where TEventHandler : class
    {
        _conventionInstanceBuilders.Add(new ConventionInstanceEventHandlerBuilder(eventHandlerInstance));
        _conventionInstanceBindingsBuilder.Add(eventHandlerInstance);
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
        var eventHandlerBuilderResults = BuildEventHandlers(_builderBindingsBuilder.Build(buildResults), _builders, eventTypes, tenantScopedProvidersFactory, buildResults);
        var conventionTypeBuilderResults = BuildEventHandlers(_conventionTypeBindingsBuilder.Build(buildResults), _conventionTypeBuilders, eventTypes, tenantScopedProvidersFactory, buildResults);
        var conventionInstanceBuilderResults = BuildEventHandlers(_conventionInstanceBindingsBuilder.Build(buildResults), _conventionInstanceBuilders, eventTypes, tenantScopedProvidersFactory, buildResults);

        var excludedIds = new HashSet<EventHandlerId>();
        var allIds = eventHandlerBuilderResults
            .Select(_ => _.Identifier)
            .Concat(conventionInstanceBuilderResults.Select(_ => _.Identifier))
            .Concat(conventionTypeBuilderResults.Select(_ => _.Identifier))
            .ToHashSet();
        
        
        foreach ()
        {
        }
    }

    static IEnumerable<EventHandlerBuildInformation<TBuilder>> BuildEventHandlers<TBuilder, TValue>(
        IUniqueBindings<EventHandlerId, TValue> uniqueBindings,
        IEnumerable<TBuilder> builders,
        IEventTypes eventTypes,
        Func<ITenantScopedProviders> tenantScopedProvidersFactory,
        IClientBuildResults buildResults)
        where TBuilder : ICanTryBuildEventHandler
        where TValue : class
        => builders
            .Where(_ => _.TryGetIdentifier(out var _))
            .Select(builder =>
            {
                IEventHandler eventHandler = default;
                builder.TryGetIdentifier(out var eventHandlerId);
                var eventHandlerBuilt = uniqueBindings.HasFor(eventHandlerId) && builder.TryBuild(eventTypes, buildResults, tenantScopedProvidersFactory, out eventHandler);
                return new EventHandlerBuildInformation<TBuilder>(eventHandlerBuilt, eventHandlerId, eventHandler, builder);
            });
    
    static bool IsEventHandler(Type type)
        => type.GetCustomAttributes(typeof(EventHandlerAttribute), true).FirstOrDefault() is EventHandlerAttribute;

    record EventHandlerBuildInformation<TBuilder>(bool Success, EventHandlerId Identifier, IEventHandler EventHandler, TBuilder Builder)
        where TBuilder : ICanTryBuildEventHandler;
}

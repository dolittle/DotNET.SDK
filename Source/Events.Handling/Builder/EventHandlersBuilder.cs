// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dolittle.SDK.Common;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Common.Model;
using Dolittle.SDK.DependencyInversion;
using Dolittle.SDK.Events.Handling.Builder.Convention.Instance;
using Dolittle.SDK.Events.Handling.Builder.Convention.Type;

namespace Dolittle.SDK.Events.Handling.Builder;

/// <summary>
/// Represents an implementation of <see cref="IEventHandlersBuilder"/>.
/// </summary>
public class EventHandlersBuilder : IEventHandlersBuilder
{
    readonly IModelBuilder _modelBuilder;
    readonly IClientBuildResults _buildResults;

    readonly DecoratedTypeBindingsToModelAdder<EventHandlerAttribute, EventHandlerModelId, EventHandlerId> _decoratedTypeBindings;

    public EventHandlersBuilder(IModelBuilder modelBuilder, IClientBuildResults buildResults)
    {
        _modelBuilder = modelBuilder;
        _buildResults = buildResults;
        _decoratedTypeBindings = new DecoratedTypeBindingsToModelAdder<EventHandlerAttribute, EventHandlerModelId, EventHandlerId>("event handler", modelBuilder, buildResults);
    }

    /// <inheritdoc />
    public IEventHandlerBuilder CreateEventHandler(EventHandlerId eventHandlerId)
    {
        var builder = new EventHandlerBuilder(eventHandlerId, _modelBuilder);
        return builder;
    }

    /// <inheritdoc />
    public IEventHandlersBuilder RegisterEventHandler<TEventHandler>()
        where TEventHandler : class
        => RegisterEventHandler(typeof(TEventHandler));

    /// <inheritdoc />
    public IEventHandlersBuilder RegisterEventHandler(Type type)
    {
        if (!_decoratedTypeBindings.TryAdd(type, out var decorator))
        {
            return this;
        }
        _modelBuilder.BindIdentifierToProcessorBuilder<ICanTryBuildEventHandler>(decorator.GetIdentifier(), new ConventionTypeEventHandlerBuilder(type, decorator));
        return this;
    }

    /// <inheritdoc />
    public IEventHandlersBuilder RegisterEventHandler<TEventHandler>(TEventHandler eventHandlerInstance)
        where TEventHandler : class
    {
        if (!_decoratedTypeBindings.TryAdd(eventHandlerInstance.GetType(), out var decorator))
        {
            return this;
        }
        _modelBuilder.BindIdentifierToProcessorBuilder<ICanTryBuildEventHandler>(decorator.GetIdentifier(), new ConventionInstanceEventHandlerBuilder(eventHandlerInstance, decorator));
        return this;
    }

    /// <inheritdoc />
    public IEventHandlersBuilder RegisterAllFrom(Assembly assembly)
    {
        var addedEventHandlerBindings = _decoratedTypeBindings.AddFromAssembly(assembly);
        foreach (var (type, decorator) in addedEventHandlerBindings)
        {
            _modelBuilder.BindIdentifierToProcessorBuilder<ICanTryBuildEventHandler>(decorator.GetIdentifier(), new ConventionTypeEventHandlerBuilder(type, decorator));
        }

        return this;
    }

    /// <summary>
    /// Build all event handlers.
    /// </summary>
    /// <param name="model">The <see cref="IModel"/>.</param>
    /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
    /// <param name="tenantScopedProvidersFactory">The <see cref="Func{TResult}"/> for getting <see cref="ITenantScopedProviders" />.</param>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    public IUnregisteredEventHandlers Build(IModel model, IEventTypes eventTypes, Func<ITenantScopedProviders> tenantScopedProvidersFactory, IClientBuildResults buildResults)
    {
        var builders = model.GetProcessorBuilderBindings<ICanTryBuildEventHandler>();
        var eventHandlers = new List<IEventHandler>();
        foreach (var builder in builders.Select(_ => _.ProcessorBuilder))
        {
            if (builder.TryBuild(eventTypes, buildResults, tenantScopedProvidersFactory, out var eventHandler))
            {
                eventHandlers.Add(eventHandler);
            }
        }
        return new UnregisteredEventHandlers(
            new UniqueBindings<EventHandlerId, IEventHandler>(eventHandlers.ToDictionary(_ => _.Identifier, _ => _)),
            model.GetProcessorBuilderBindings<ConventionInstanceEventHandlerBuilder>(),
            model.GetProcessorBuilderBindings<ConventionTypeEventHandlerBuilder>());
    }

}

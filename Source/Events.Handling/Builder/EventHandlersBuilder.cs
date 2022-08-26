// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using Dolittle.SDK.Common;
using Dolittle.SDK.ApplicationModel.ClientSetup;
using Dolittle.SDK.ApplicationModel;
using Dolittle.SDK.Events.Handling.Builder.Convention.Instance;
using Dolittle.SDK.Events.Handling.Builder.Convention.Type;

namespace Dolittle.SDK.Events.Handling.Builder;

/// <summary>
/// Represents an implementation of <see cref="IEventHandlersBuilder"/>.
/// </summary>
public class EventHandlersBuilder : IEventHandlersBuilder
{
    readonly IModelBuilder _modelBuilder;
    readonly DecoratedTypeBindingsToModelAdder<EventHandlerAttribute, EventHandlerModelId, EventHandlerId> _decoratedTypeBindings;

    /// <summary>
    /// Initializes an instance of the <see cref="EventHandlersBuilder"/> class.
    /// </summary>
    /// <param name="modelBuilder">The <see cref="IModelBuilder"/>.</param>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    public EventHandlersBuilder(IModelBuilder modelBuilder, IClientBuildResults buildResults)
    {
        _modelBuilder = modelBuilder;
        _decoratedTypeBindings = new DecoratedTypeBindingsToModelAdder<EventHandlerAttribute, EventHandlerModelId, EventHandlerId>("event handler", modelBuilder, buildResults);
    }

    /// <inheritdoc />
    public IEventHandlerBuilder Create(EventHandlerId eventHandlerId)
    {
        var builder = new EventHandlerBuilder(eventHandlerId, _modelBuilder);
        return builder;
    }

    /// <inheritdoc />
    public IEventHandlersBuilder Register<TEventHandler>()
        where TEventHandler : class
        => Register(typeof(TEventHandler));

    /// <inheritdoc />
    public IEventHandlersBuilder Register(Type type)
    {
        if (!_decoratedTypeBindings.TryAdd(type, out var binding))
        {
            return this;
        }
        _modelBuilder.BindIdentifierToProcessorBuilder<ConventionTypeEventHandlerBuilder, EventHandlerModelId, EventHandlerId>(binding.Identifier, new ConventionTypeEventHandlerBuilder(binding));
        return this;
    }

    /// <inheritdoc />
    public IEventHandlersBuilder Register<TEventHandler>(TEventHandler eventHandlerInstance)
        where TEventHandler : class
    {
        if (!_decoratedTypeBindings.TryAdd(eventHandlerInstance.GetType(), out var binding))
        {
            return this;
        }
        _modelBuilder.BindIdentifierToProcessorBuilder<ConventionInstanceEventHandlerBuilder, EventHandlerModelId, EventHandlerId>(binding.Identifier, new ConventionInstanceEventHandlerBuilder(eventHandlerInstance, binding));
        return this;
    }

    /// <inheritdoc />
    public IEventHandlersBuilder RegisterAllFrom(Assembly assembly)
    {
        var addedEventHandlerBindings = _decoratedTypeBindings.AddFromAssembly(assembly);
        foreach (var binding in addedEventHandlerBindings)
        {
            _modelBuilder.BindIdentifierToProcessorBuilder<ConventionTypeEventHandlerBuilder, EventHandlerModelId, EventHandlerId>(binding.Identifier, new ConventionTypeEventHandlerBuilder(binding));
        }

        return this;
    }

    /// <summary>
    /// Build all event handlers.
    /// </summary>
    /// <param name="applicationModel">The <see cref="IApplicationModel"/>.</param>
    /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    public static IUnregisteredEventHandlers Build(IApplicationModel applicationModel, IEventTypes eventTypes, IClientBuildResults buildResults)
    {
        var eventHandlers = new UniqueBindings<EventHandlerModelId, IEventHandler>();
        foreach (var (identifier, builder) in applicationModel.GetProcessorBuilderBindings<ConventionTypeEventHandlerBuilder, EventHandlerModelId, EventHandlerId>())
        {
            if (builder.TryBuild(identifier, eventTypes, buildResults, out var eventHandler))
            {
                eventHandlers.Add(identifier, eventHandler);
            }
            else
            {
                buildResults.AddFailure($"Failed to build Event Handler for '{builder.EventHandlerType}'. It will not be registered");
            }
        }
        foreach (var (identifier, builder) in applicationModel.GetProcessorBuilderBindings<ConventionInstanceEventHandlerBuilder, EventHandlerModelId, EventHandlerId>())
        {
            if (builder.TryBuild(identifier, eventTypes, buildResults, out var eventHandler))
            {
                eventHandlers.Add(identifier, eventHandler);
            }
            else
            {
                buildResults.AddFailure($"Failed to build Event Handler for instance of '{builder.EventHandlerType}'. It will not be registered");
            }
        }
        foreach (var (identifier, builder) in applicationModel.GetProcessorBuilderBindings<EventHandlerBuilder, EventHandlerModelId, EventHandlerId>())
        {
            if (builder.TryBuild(identifier, eventTypes, buildResults, out var eventHandler))
            {
                eventHandlers.Add(identifier, eventHandler);
            }
            
            else
            {
                buildResults.AddFailure($"Failed to build Event Handler '{identifier.Id}' in Scope '{identifier.Scope}'. It will not be registered");
            }
        }
        return new UnregisteredEventHandlers(
            eventHandlers,
            applicationModel.GetProcessorBuilderBindings<ConventionInstanceEventHandlerBuilder, EventHandlerModelId, EventHandlerId>(),
            applicationModel.GetProcessorBuilderBindings<ConventionTypeEventHandlerBuilder, EventHandlerModelId, EventHandlerId>());
    }

}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reflection;
using Dolittle.SDK.Artifacts;
using Dolittle.SDK.Common;
using Dolittle.SDK.ApplicationModel.ClientSetup;
using Dolittle.SDK.ApplicationModel;

namespace Dolittle.SDK.Events.Builders;

/// <summary>
/// Represents a system that registers <see cref="Type" /> to <see cref="EventType" /> associations to an <see cref="IEventTypes" /> instance.
/// </summary>
public class EventTypesBuilder : IEventTypesBuilder
{
    readonly IModelBuilder _modelBuilder;
    readonly DecoratedTypeBindingsToModelAdder<EventTypeAttribute, EventType, EventTypeId> _decoratedTypeBindings;

    /// <summary>
    /// Initializes an instance of the <see cref="EventTypesBuilder"/> class.
    /// </summary>
    /// <param name="modelBuilder">The <see cref="IModelBuilder"/>.</param>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    public EventTypesBuilder(IModelBuilder modelBuilder, IClientBuildResults buildResults)
    {
        _modelBuilder = modelBuilder;
        _decoratedTypeBindings = new DecoratedTypeBindingsToModelAdder<EventTypeAttribute, EventType, EventTypeId>("event type", modelBuilder, buildResults);
    }

    /// <inheritdoc />
    public IEventTypesBuilder Associate<T>(EventType eventType)
        where T : class
        => Associate(typeof(T), eventType);

    /// <inheritdoc />
    public IEventTypesBuilder Associate(Type type, EventType eventType)
    {
        Register(type);
        _modelBuilder.BindIdentifierToType<EventType, EventTypeId>(eventType, type);
        return this;
    }

    /// <inheritdoc />
    public IEventTypesBuilder Associate<T>(EventTypeId eventTypeId, IdentifierAlias alias = default)
        where T : class
        => Associate(typeof(T), eventTypeId, alias);

    /// <inheritdoc />
    public IEventTypesBuilder Associate(Type type, EventTypeId eventTypeId, IdentifierAlias alias = default)
        => Associate(type, new EventType(eventTypeId, alias: alias));

    /// <inheritdoc />
    public IEventTypesBuilder Associate<T>(EventTypeId eventTypeId, Generation generation, IdentifierAlias alias = default)
        where T : class
        => Associate(typeof(T), eventTypeId, generation, alias);

    /// <inheritdoc />
    public IEventTypesBuilder Associate(Type type, EventTypeId eventTypeId, Generation generation, IdentifierAlias alias = default)
        => Associate(type, new EventType(eventTypeId, generation, alias));
    
    /// <inheritdoc />
    public IEventTypesBuilder Register<T>()
        where T : class
        => Register(typeof(T));

    /// <inheritdoc />
    public IEventTypesBuilder Register(Type type)
    {
        _decoratedTypeBindings.TryAdd(type, out _);
        return this;
    }

    /// <inheritdoc />
    public IEventTypesBuilder RegisterAllFrom(Assembly assembly)
    {
        _decoratedTypeBindings.AddFromAssembly(assembly);
        return this;
    }

    /// <summary>
    /// Builds the <see cref="IEventTypes"/>.
    /// </summary>
    public static IUnregisteredEventTypes Build(IApplicationModel applicationModel)
    {
        var bindings = applicationModel.GetTypeBindings<EventType, EventTypeId>();
        return new UnregisteredEventTypes(new UniqueBindings<EventType, Type>(bindings.ToDictionary(_ => _.Identifier, _ => _.Type)));
    }
}

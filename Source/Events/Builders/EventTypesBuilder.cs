// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using Dolittle.SDK.Artifacts;
using Dolittle.SDK.Common.ClientSetup;

namespace Dolittle.SDK.Events.Builders;

/// <summary>
/// Represents a system that registers <see cref="Type" /> to <see cref="EventType" /> associations to an <see cref="IEventTypes" /> instance.
/// </summary>
public class EventTypesBuilder : IEventTypesBuilder
{
    readonly ClientArtifactsBuilder<EventType, EventTypeId, EventTypeAttribute> _artifactsBuilder = new();

    /// <inheritdoc />
    public IEventTypesBuilder Associate<T>(EventType eventType)
        where T : class
        => Associate(typeof(T), eventType);

    /// <inheritdoc />
    public IEventTypesBuilder Associate(Type type, EventType eventType)
    {
        _artifactsBuilder.Add(eventType, type);
        return this;
    }

    /// <inheritdoc />
    public IEventTypesBuilder Associate<T>(EventTypeId eventTypeId, EventTypeAlias alias = default)
        where T : class
        => Associate(typeof(T), eventTypeId, alias);

    /// <inheritdoc />
    public IEventTypesBuilder Associate(Type type, EventTypeId eventTypeId, EventTypeAlias alias = default)
        => Associate(type, new EventType(eventTypeId, alias));

    /// <inheritdoc />
    public IEventTypesBuilder Associate<T>(EventTypeId eventTypeId, Generation generation, EventTypeAlias alias = default)
        where T : class
        => Associate(typeof(T), eventTypeId, generation, alias);

    /// <inheritdoc />
    public IEventTypesBuilder Associate(Type type, EventTypeId eventTypeId, Generation generation, EventTypeAlias alias = default)
        => Associate(type, new EventType(eventTypeId, generation, alias));
    
    /// <inheritdoc />
    public IEventTypesBuilder Register<T>()
        where T : class
        => Register(typeof(T));

    /// <inheritdoc />
    public IEventTypesBuilder Register(Type type)
    {
        _artifactsBuilder.Add(type);
        return this;
    }

    /// <inheritdoc />
    public IEventTypesBuilder RegisterAllFrom(Assembly assembly)
    {
        _artifactsBuilder.AddAllFrom(assembly);
        return this;
    }

    /// <summary>
    /// Builds the <see cref="IEventTypes"/>.
    /// </summary>
    public IUnregisteredEventTypes Build(IClientBuildResults buildResults)
        => new UnregisteredEventTypes(_artifactsBuilder.Build(buildResults));
}

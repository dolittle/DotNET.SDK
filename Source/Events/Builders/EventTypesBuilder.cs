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
public class EventTypesBuilder : ClientArtifactsBuilder<EventType, EventTypeId, EventTypeAttribute>, IEventTypesBuilder
{
    /// <summary>
    /// Initializes an instance of the <see cref="EventTypesBuilder"/> class.
    /// </summary>
    /// <param name="clientBuildResults"></param>
    public EventTypesBuilder(IClientBuildResults clientBuildResults)
        : base(clientBuildResults)
    {
    }

    /// <inheritdoc />
    public IEventTypesBuilder Associate<T>(EventType eventType)
        where T : class
        => Associate(typeof(T), eventType);

    /// <inheritdoc />
    public new IEventTypesBuilder Associate(Type type, EventType eventType)
    {
        base.Associate(type, eventType);
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
    public new IEventTypesBuilder Register(Type type)
    {
        base.Register(type);
        return this;
    }

    /// <inheritdoc />
    public new IEventTypesBuilder RegisterAllFrom(Assembly assembly)
    {
        base.RegisterAllFrom(assembly);
        return this;
    }

    /// <summary>
    /// Builds the <see cref="IEventTypes"/>.
    /// </summary>
    public new IUnregisteredEventTypes Build()
        => new UnregisteredEventTypes(base.Build());

    /// <inheritdoc />
    protected override bool TryGetArtifactFromAttribute(Type type, EventTypeAttribute attribute, out EventType artifact)
    {
        if (!attribute.HasAlias)
        {
            artifact = new EventType(attribute.Identifier, attribute.Generation, type.Name);
            return true;
        }

        artifact = new EventType(attribute.Identifier, attribute.Generation, attribute.Alias);
        return true;
    }
}

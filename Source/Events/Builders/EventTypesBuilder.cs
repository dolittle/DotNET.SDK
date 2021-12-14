// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dolittle.SDK.Artifacts;


namespace Dolittle.SDK.Events.Builders;

/// <summary>
/// Represents a system that registers <see cref="Type" /> to <see cref="EventType" /> associations to an <see cref="IEventTypes" /> instance.
/// </summary>
public class EventTypesBuilder : IEventTypesBuilder
{
    readonly Dictionary<Type, EventType> _associations = new();

    /// <inheritdoc />
    public IEventTypesBuilder Associate<T>(EventType eventType)
        where T : class
        => Associate(typeof(T), eventType);

    /// <inheritdoc />
    public IEventTypesBuilder Associate(Type type, EventType eventType)
    {
        AddAssociation(type, eventType);
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
        ThrowIfTypeIsMissingEventTypeAttribute(type);
        TryGetEventTypeFromAttribute(type, out var eventType);
        AddAssociation(type, eventType);
        return this;
    }

    /// <inheritdoc />
    public IEventTypesBuilder RegisterAllFrom(Assembly assembly)
    {
        foreach (var type in assembly.ExportedTypes)
        {
            if (IsEventType(type))
            {
                Register(type);
            }
        }

        return this;
    }

    /// <summary>
    /// Builds the <see cref="IEventTypes"/>.
    /// </summary>
    public IUnregisteredEventTypes Build()
    {
        var eventTypes = new UnregisteredEventTypes();
        foreach (var (type, eventType) in _associations)
        {
            eventTypes.Associate(type, eventType);
        }
        return eventTypes;
    }

    static bool IsEventType(Type type)
        => type.GetCustomAttributes(typeof(EventTypeAttribute), true).FirstOrDefault() is EventTypeAttribute;

    static void ThrowIfAttributeSpecifiesADifferentEventType(Type type, EventType providedType)
    {
        if (TryGetEventTypeFromAttribute(type, out var attributeType) && attributeType != providedType)
        {
            throw new ProvidedEventTypeDoesNotMatchEventTypeFromAttribute(providedType, attributeType, type);
        }
    }

    static bool TryGetEventTypeFromAttribute(Type type, out EventType eventType)
    {
        if (Attribute.GetCustomAttribute(type, typeof(EventTypeAttribute)) is EventTypeAttribute attribute)
        {
            if (!attribute.HasAlias)
            {
                eventType = new EventType(attribute.Identifier, attribute.Generation, type.Name);
                return true;
            }

            eventType = new EventType(attribute.Identifier, attribute.Generation, attribute.Alias);
            return true;
        }

        eventType = default;
        return false;
    }

    void AddAssociation(Type type, EventType eventType)
    {
        ThrowIfAttributeSpecifiesADifferentEventType(type, eventType);
        _associations[type] = eventType;
    }

    void ThrowIfTypeIsMissingEventTypeAttribute(Type type)
    {
        if (!TryGetEventTypeFromAttribute(type, out _))
        {
            throw new TypeIsMissingEventTypeAttribute(type);
        }
    }
}

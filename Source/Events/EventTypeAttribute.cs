// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Artifacts;
using Dolittle.SDK.Common.Model;

namespace Dolittle.SDK.Events;

/// <summary>
/// Decorates a class to indicate the <see cref="EventType" /> of an event class.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class EventTypeAttribute : Attribute, IDecoratedTypeDecorator<EventType>
{
    readonly EventTypeId _eventTypeId;
    readonly Generation _generation;
    readonly string? _alias;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventTypeAttribute"/> class.
    /// </summary>
    /// <param name="eventTypeId">The unique identifier of the <see cref="EventType" />.</param>
    /// <param name="generation">The generation of the <see cref="EventType" />..</param>
    /// <param name="alias">The alias for the <see cref="EventType"/>.</param>
    public EventTypeAttribute(string eventTypeId, uint generation = 0, string? alias = null)
    {
        _eventTypeId = eventTypeId;
        _generation = generation == 0 ? Generation.First : new Generation(generation);
        _alias = alias;
    }
    
    /// <inheritdoc />
    public EventType GetIdentifier(Type decoratedType)
        => new(_eventTypeId, _generation, _alias ?? decoratedType.Name);
}

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
    /// <summary>
    /// Initializes a new instance of the <see cref="EventTypeAttribute"/> class.
    /// </summary>
    /// <param name="eventTypeId">The unique identifier of the <see cref="EventType" />.</param>
    /// <param name="generation">The generation of the <see cref="EventType" />..</param>
    /// <param name="alias">The alias for the <see cref="EventType"/>.</param>
    public EventTypeAttribute(string eventTypeId, uint generation = 0, string alias = default)
    {
        Identifier = Guid.Parse(eventTypeId);
        Generation = generation == 0 ? Generation.First : new Generation(generation);
        if (alias == default)
        {
            return;
        }
        Alias = alias;
        HasAlias = true;
    }

    /// <summary>
    /// Gets the unique identifier for this event type.
    /// </summary>
    public EventTypeId Identifier { get; }

    /// <summary>
    /// Gets the generation for this event type.
    /// </summary>
    public Generation Generation { get;  }

    /// <summary>
    /// Gets the <see cref="EventTypeAlias"/>.
    /// </summary>
    public EventTypeAlias Alias { get; }

    /// <summary>
    /// Gets a value indicating whether this event type has an alias.
    /// </summary>
    public bool HasAlias { get; }


    /// <inheritdoc />
    public EventType GetIdentifier() => HasAlias
        ? new EventType(Identifier, Generation, Alias)
        : new EventType(Identifier, Generation);
}

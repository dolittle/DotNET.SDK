// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;

namespace Dolittle.SDK.Aggregates;

/// <summary>
/// Represents an uncommitted event that is applied on an aggregate.
/// </summary>
public class AppliedEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppliedEvent"/> class.
    /// </summary>
    /// <param name="event">The event content.</param>
    /// <param name="eventType">The <see cref="EventType" />.</param>
    /// <param name="isPublic">Whether the event is public or not.</param>
    public AppliedEvent(object @event, EventType? eventType, bool isPublic)
    {
        Event = @event;
        EventType = eventType;
        Public = isPublic;
    }

    /// <summary>
    /// Gets the event content.
    /// </summary>
    public object Event { get; }

    /// <summary>
    /// Gets the event's <see cref="EventType" />.
    /// </summary>
    public EventType? EventType { get; }

    /// <summary>
    /// Gets a value indicating whether this event is public or not.
    /// </summary>
    public bool Public { get; }

    /// <summary>
    /// Gets a value indicating whether this applied event has been given an <see cref="EventType" />.
    /// </summary>
    public bool HasEventType => EventType != default;
}

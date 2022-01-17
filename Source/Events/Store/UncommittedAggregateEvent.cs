// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Events.Store;

/// <summary>
/// Represents an aggregate event that has not been committed to the Event Store.
/// </summary>
public class UncommittedAggregateEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UncommittedAggregateEvent"/> class.
    /// </summary>
    /// <param name="eventType">The <see cref="EventType"/> the Event is associated with.</param>
    /// <param name="content">The content of the Event.</param>
    /// <param name="isPublic">Whether the event is public or not.</param>
    public UncommittedAggregateEvent(EventType eventType, object content, bool isPublic)
    {
        ThrowIfEventTypeIsNull(eventType);
        ThrowIfContentIsNull(content);

        EventType = eventType;
        Content = content;
        IsPublic = isPublic;
    }

    /// <summary>
    /// Gets the Artifact this event is associated with.
    /// </summary>
    public EventType EventType { get; }

    /// <summary>
    /// Gets the content of the event.
    /// </summary>
    public object Content { get; }

    /// <summary>
    /// Gets a value indicating whether the Event is public or not.
    /// </summary>
    public bool IsPublic { get; }

    void ThrowIfEventTypeIsNull(EventType eventType)
    {
        if (eventType == null) throw new EventTypeCannotBeNull();
    }

    void ThrowIfContentIsNull(object content)
    {
        if (content == null) throw new EventContentCannotBeNull();
    }
}
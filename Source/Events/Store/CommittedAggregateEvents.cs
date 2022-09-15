// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Dolittle.SDK.Events.Store;

/// <summary>
/// Represents a sequence of Events applied by an AggregateRoot to an Event Source that have been committed to the Event Store.
/// </summary>
public class CommittedAggregateEvents : IReadOnlyList<CommittedAggregateEvent>
{
    readonly ImmutableList<CommittedAggregateEvent> _events;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommittedAggregateEvents"/> class.
    /// </summary>
    /// <param name="eventSource">The <see cref="EventSourceId"/> that the Events were applied to.</param>
    /// <param name="aggregateRootId">The <see cref="AggregateRootId"/> of the Aggregate Root that applied the Events to the Event Source.</param>
    /// <param name="currentAggregateRootVersion">The current <see cref="AggregateRootVersion"/> after all events was applied.</param>
    /// <param name="events">The <see cref="CommittedAggregateEvent">events</see>.</param>
    public CommittedAggregateEvents(EventSourceId eventSource, AggregateRootId aggregateRootId, AggregateRootVersion currentAggregateRootVersion, IReadOnlyList<CommittedAggregateEvent> events)
    {
        ThrowIfEventsSourceIdIsNull(eventSource);
        ThrowIfAggregateRootIdIsNull(aggregateRootId);

        EventSource = eventSource;
        AggregateRoot = aggregateRootId;
        AggregateRootVersion = currentAggregateRootVersion;
        for (var i = 0; i < events.Count; i++)
        {
            var @event = events[i];
            ThrowIfEventIsNull(@event);
            ThrowIfEventWasAppliedToOtherEventSource(@event);
            ThrowIfEventWasAppliedByOtherAggregateRoot(@event);
            if (i <= 0)
            {
                continue;
            }
            ThrowIfAggregateRootVersionIsOutOfOrder(@event, events[i - 1]);
            ThrowIfEventLogVersionIsOutOfOrder(@event, events[i - 1]);
        }

        _events = ImmutableList<CommittedAggregateEvent>.Empty.AddRange(events);
    }

    /// <summary>
    /// Gets the <see cref="EventSourceId"/> that the Events were applied to.
    /// </summary>
    public EventSourceId EventSource { get; }

    /// <summary>
    /// Gets the <see cref="AggregateRootId"/> of the Aggregate Root that applied the Events to the Event Source.
    /// </summary>
    public AggregateRootId AggregateRoot { get; }

    /// <summary>
    /// Gets the <see cref="AggregateRootVersion"/>  of the Aggregate Root after all the Events was applied.
    /// </summary>
    public AggregateRootVersion AggregateRootVersion { get; }

    /// <summary>
    /// Gets a value indicating whether or not there are any events in the committed sequence.
    /// </summary>
    public bool HasEvents => Count > 0;

    /// <inheritdoc/>
    public int Count => _events.Count;

    /// <inheritdoc/>
    public CommittedAggregateEvent this[int index] => _events[index];

    /// <inheritdoc/>
    public IEnumerator<CommittedAggregateEvent> GetEnumerator() => _events.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => _events.GetEnumerator();

    static void ThrowIfEventsSourceIdIsNull(EventSourceId eventSource)
    {
        if (eventSource == null)
        {
            throw new EventSourceIdCannotBeNull();
        }
    }

    static void ThrowIfAggregateRootIdIsNull(AggregateRootId aggregateRootId)
    {
        if (aggregateRootId == null)
        {
            throw new AggregateRootIdCannotBeNull();
        }
    }

    static void ThrowIfEventIsNull(CommittedAggregateEvent @event)
    {
        if (@event == null)
        {
            throw new EventCannotBeNull();
        }
    }

    void ThrowIfEventWasAppliedToOtherEventSource(CommittedAggregateEvent @event)
    {
        if (@event.EventSource != EventSource)
        {
            throw new EventWasAppliedToOtherEventSource(@event.EventSource, EventSource);
        }
    }

    void ThrowIfEventWasAppliedByOtherAggregateRoot(CommittedAggregateEvent @event)
    {
        if (@event.AggregateRoot != AggregateRoot)
        {
            throw new EventWasAppliedByOtherAggregateRoot(@event.AggregateRoot, AggregateRoot);
        }
    }

    static void ThrowIfAggregateRootVersionIsOutOfOrder(CommittedAggregateEvent @event, CommittedAggregateEvent previousEvent)
    {
        if (@event.AggregateRootVersion <= previousEvent.AggregateRootVersion)
        {
            throw new AggregateRootVersionIsOutOfOrder(@event.AggregateRootVersion, previousEvent.AggregateRootVersion);
        }
    }

    static void ThrowIfEventLogVersionIsOutOfOrder(CommittedAggregateEvent @event, CommittedAggregateEvent previousEvent)
    {
        if (@event.EventLogSequenceNumber <= previousEvent.EventLogSequenceNumber)
        {
            throw new EventLogSequenceNumberIsOutOfOrder(@event.EventLogSequenceNumber, previousEvent.EventLogSequenceNumber);
        }
    }
}

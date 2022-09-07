// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using Dolittle.SDK.Artifacts;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Store;

namespace Dolittle.SDK.Aggregates;

/// <summary>
/// Represents the aggregate root.
/// </summary>
public class AggregateRoot
{
    readonly List<AppliedEvent> _appliedEvents = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRoot"/> class.
    /// </summary>
    /// <param name="eventSourceId">The <see cref="Events.EventSourceId" />.</param>
    public AggregateRoot(EventSourceId eventSourceId)
    {
        EventSourceId = eventSourceId;
        Version = AggregateRootVersion.Initial;
        IsStateless = this.IsStateless();
    }

    /// <summary>
    /// Gets the current <see cref="AggregateRootVersion" />.
    /// </summary>
    public AggregateRootVersion Version { get; private set; }

    /// <summary>
    /// Gets the <see cref="Events.EventSourceId" /> that the <see cref="AggregateRoot" /> applies events to.
    /// </summary>
    public EventSourceId EventSourceId { get; }

    /// <summary>
    /// Gets the <see cref="IEnumerable{T}" /> of applied events to commit.
    /// </summary>
    public IEnumerable<AppliedEvent> AppliedEvents => _appliedEvents;
    
    /// <summary>
    /// Gets a value indicating whether this aggregate root is stateless or not.
    /// </summary>
    public bool IsStateless { get; }

    /// <summary>
    /// Apply the event to the <see cref="AggregateRoot" /> so that it will be committed to the <see cref="IEventStore" />
    /// when <see cref="IAggregateRootOperations{TAggregate}.Perform(Action{TAggregate},CancellationToken)"/> is invoked on the <see cref="AggregateRoot" />.
    /// </summary>
    /// <remarks>The state of the <see cref="AggregateRoot" /> is changed by calling the appropriate On-methods for the applied events.</remarks>
    /// <param name="event">The event to apply.</param>
    public void Apply(object @event)
        => Apply(@event, default, false);

    /// <summary>
    /// Apply the public event to the <see cref="AggregateRoot" /> so that it will be committed to the <see cref="IEventStore" />
    /// when <see cref="IAggregateRootOperations{TAggregate}.Perform(Action{TAggregate},CancellationToken)"/> is invoked on the <see cref="AggregateRoot" />.
    /// </summary>
    /// <remarks>The state of the <see cref="AggregateRoot" /> is changed by calling the appropriate On-methods for the applied events.</remarks>
    /// <param name="event">The event to apply.</param>
    public void ApplyPublic(object @event)
        => Apply(@event, default, true);

    /// <summary>
    /// Apply the event to the <see cref="AggregateRoot" /> so that it will be committed to the <see cref="IEventStore" />
    /// when <see cref="IAggregateRootOperations{TAggregate}.Perform(Action{TAggregate},CancellationToken)"/> is invoked on the <see cref="AggregateRoot" />.
    /// </summary>
    /// <remarks>The state of the <see cref="AggregateRoot" /> is changed by calling the appropriate On-methods for the applied events.</remarks>
    /// <param name="event">The event to apply.</param>
    /// <param name="eventTypeId">The <see cref="EventTypeId" />.</param>
    public void Apply(object @event, EventTypeId eventTypeId)
        => Apply(@event, new EventType(eventTypeId));

    /// <summary>
    /// Apply the public event to the <see cref="AggregateRoot" /> so that it will be committed to the <see cref="IEventStore" />
    /// when <see cref="IAggregateRootOperations{TAggregate}.Perform(Action{TAggregate},CancellationToken)"/> is invoked on the <see cref="AggregateRoot" />.
    /// </summary>
    /// <remarks>The state of the <see cref="AggregateRoot" /> is changed by calling the appropriate On-methods for the applied events.</remarks>
    /// <param name="event">The event to apply.</param>
    /// <param name="eventTypeId">The <see cref="EventTypeId" />.</param>
    public void ApplyPublic(object @event, EventTypeId eventTypeId)
        => ApplyPublic(@event, new EventType(eventTypeId));

    /// <summary>
    /// Apply the event to the <see cref="AggregateRoot" /> so that it will be committed to the <see cref="IEventStore" />
    /// when <see cref="IAggregateRootOperations{TAggregate}.Perform(Action{TAggregate},CancellationToken)"/> is invoked on the <see cref="AggregateRoot" />.
    /// </summary>
    /// <remarks>The state of the <see cref="AggregateRoot" /> is changed by calling the appropriate On-methods for the applied events.</remarks>
    /// <param name="event">The event to apply.</param>
    /// <param name="eventTypeId">The <see cref="EventTypeId" /> of the event type.</param>
    /// <param name="generation">The <see cref="Generation" /> of the event type.</param>
    public void Apply(object @event, EventTypeId eventTypeId, Generation generation)
        => Apply(@event, new EventType(eventTypeId, generation));

    /// <summary>
    /// Apply the public event to the <see cref="AggregateRoot" /> so that it will be committed to the <see cref="IEventStore" />
    /// when <see cref="IAggregateRootOperations{TAggregate}.Perform(Action{TAggregate},CancellationToken)"/> is invoked on the <see cref="AggregateRoot" />.
    /// </summary>
    /// <remarks>The state of the <see cref="AggregateRoot" /> is changed by calling the appropriate On-methods for the applied events.</remarks>
    /// <param name="event">The event to apply.</param>
    /// <param name="eventTypeId">The <see cref="EventTypeId" /> of the event type.</param>
    /// <param name="generation">The <see cref="Generation" /> of the event type.</param>
    public void ApplyPublic(object @event, EventTypeId eventTypeId, Generation generation)
        => ApplyPublic(@event, new EventType(eventTypeId, generation));

    /// <summary>
    /// Apply the event to the <see cref="AggregateRoot" /> so that it will be committed to the <see cref="IEventStore" />
    /// when <see cref="IAggregateRootOperations{TAggregate}.Perform(Action{TAggregate},CancellationToken)"/> is invoked on the <see cref="AggregateRoot" />.
    /// </summary>
    /// <remarks>The state of the <see cref="AggregateRoot" /> is changed by calling the appropriate On-methods for the applied events.</remarks>
    /// <param name="event">The event to apply.</param>
    /// <param name="eventType">The <see cref="EventType" />.</param>
    public void Apply(object @event, EventType eventType)
        => Apply(@event, eventType, false);

    /// <summary>
    /// Apply the public event to the <see cref="AggregateRoot" /> so that it will be committed to the <see cref="IEventStore" />
    /// when <see cref="IAggregateRootOperations{TAggregate}.Perform(Action{TAggregate},CancellationToken)"/> is invoked on the <see cref="AggregateRoot" />.
    /// </summary>
    /// <remarks>The state of the <see cref="AggregateRoot" /> is changed by calling the appropriate On-methods for the applied events.</remarks>
    /// <param name="event">The event to apply.</param>
    /// <param name="eventType">The <see cref="EventType" />.</param>
    public void ApplyPublic(object @event, EventType eventType)
        => Apply(@event, eventType, true);

    /// <summary>
    /// Re-apply events from the Event Store.
    /// </summary>
    /// <remarks>
    /// This method is not supposed to be used by application code.
    /// This is for internal rehydration of aggregate roots.
    /// </remarks>
    /// <param name="events">Sequence that contains the events to re-apply.</param>
    [Obsolete("This should eventually be removed from the AggregateRoot interface and replaced by an internal method")]
    public virtual void ReApply(CommittedAggregateEvents events)
    {
        ThrowIfEventWasAppliedToOtherEventSource(events);
        ThrowIfEventWasAppliedByOtherAggregateRoot(events);
        if (IsStateless || !events.HasEvents)
        {
            Version = events.AggregateRootVersion;
            return;
        }

        foreach (var @event in events)
        {
            Version = @event.AggregateRootVersion + 1;
            InvokeOnMethod(@event.Content);
        }
    }
    
    internal virtual void Rehydrate(CommittedAggregateEvents events)
    {
        ThrowIfEventWasAppliedToOtherEventSource(events);
        ThrowIfEventWasAppliedByOtherAggregateRoot(events);
        if (IsStateless || !events.HasEvents)
        {
            Version = events.AggregateRootVersion;
            return;
        }

        foreach (var @event in events)
        {
            Version = @event.AggregateRootVersion + 1;
            InvokeOnMethod(@event.Content);
        }
    }
    
    void Apply(object @event, EventType eventType, bool isPublic)
    {
        if (@event == null)
        {
            throw new EventContentCannotBeNull();
        }

        _appliedEvents.Add(new AppliedEvent(@event, eventType, isPublic));
        Version++;
        InvokeOnMethod(@event);
    }

    void InvokeOnMethod(object @event)
    {
        if (this.TryGetOnMethod(@event, out var handleMethod))
        {
            handleMethod.Invoke(this, new[] { @event });
        }
    }

    void ThrowIfEventWasAppliedByOtherAggregateRoot(CommittedAggregateEvents events)
    {
        var aggregateRootId = this.GetAggregateRootId();
        if (events.AggregateRoot != this.GetAggregateRootId())
        {
            throw new EventWasAppliedByOtherAggregateRoot(events.AggregateRoot, aggregateRootId);
        }
    }

    void ThrowIfEventWasAppliedToOtherEventSource(CommittedAggregateEvents events)
    {
        if (events.EventSource != EventSourceId)
        {
            throw new EventWasAppliedToOtherEventSource(events.EventSource, EventSourceId);
        }
    }
}

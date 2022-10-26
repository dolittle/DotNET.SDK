// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Aggregates.Internal;
using Dolittle.SDK.Artifacts;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Store;

namespace Dolittle.SDK.Aggregates;

/// <summary>
/// Represents the aggregate root.
/// </summary>
public abstract class AggregateRoot
{
    readonly List<AppliedEvent> _appliedEvents = new();
    EventSourceId? _eventSourceId;
    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRoot"/> class.
    /// </summary>
    /// <param name="eventSourceId">The <see cref="Events.EventSourceId" />.</param>
    [Obsolete("This base constructor is deprecated and only used to set the EventSourceId property so that it could be used in the constructor")]
    protected AggregateRoot(EventSourceId eventSourceId)
        : this()
    {
        EventSourceId = eventSourceId;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRoot"/> class.
    /// </summary>
    protected AggregateRoot()
    {
        AggregateRootId = this.GetAggregateRootId();
        Version = AggregateRootVersion.Initial;
        IsStateless = this.IsStateless();
    }

    /// <summary>
    /// Gets the current <see cref="AggregateRootVersion" />.
    /// </summary>
    public AggregateRootVersion Version { get; private set; }

    /// <summary>
    /// Gets the <see cref="Events.AggregateRootId"/>.
    /// </summary>
    public AggregateRootId AggregateRootId { get; }

    /// <summary>
    /// Gets the <see cref="Events.EventSourceId" /> that the <see cref="AggregateRoot" /> applies events to.
    /// </summary>
    [Obsolete("This will eventually be marked as internal. If you need to know the event source id in the aggregate root then include it in the constructor and keep it as a field")]
    public EventSourceId EventSourceId
    {
        get => _eventSourceId ?? throw new EventSourceIdOnAggregateRootNotReady(GetType());
        internal set
        {
            if (_eventSourceId is null)
            {
                _eventSourceId = value;
            }
            else if (_eventSourceId.Value != value.Value)
            {
                throw new CannotChangeEventSourceIdForAggregateRoot(GetType(), _eventSourceId, value);
            }
        }
    }

    /// <summary>
    /// Gets the <see cref="IEnumerable{T}" /> of applied events to commit.
    /// </summary>
    public IEnumerable<AppliedEvent> AppliedEvents => _appliedEvents;
    
    bool IsStateless { get; }

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
    /// Rehydrates the aggregate root with the <see cref="CommittedAggregateEvents"/> for this aggregate.
    /// </summary>
    /// <param name="batches">The <see cref="IAsyncEnumerator{T}"/> batches of <see cref="CommittedAggregateEvents"/> to rehydrate with.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used for cancelling the rehydration.</param>
    public async Task Rehydrate(IAsyncEnumerable<CommittedAggregateEvents> batches, CancellationToken cancellationToken)
    {
        var hasBatches = false;
        await foreach (var batch in batches.WithCancellation(cancellationToken))
        {
            hasBatches = true;
            ThrowIfEventWasAppliedToOtherEventSource(batch);
            ThrowIfEventWasAppliedByOtherAggregateRoot(batch);
            if (IsStateless)
            {
                Version = batch.AggregateRootVersion;
                break;
            }
            foreach (var @event in batch)
            {
                Version = @event.AggregateRootVersion + 1;
                InvokeOnMethod(@event.Content);
            }
        }
        if (!hasBatches)
        {
            throw new NoCommittedAggregateEventsBatches(AggregateRootId, EventSourceId);
        }
        cancellationToken.ThrowIfCancellationRequested();
    }


    void Apply(object @event, EventType? eventType, bool isPublic)
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

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Events.Store.Builders;

namespace Dolittle.SDK.Events.Store;

/// <summary>
/// Represents an implementation of <see cref="IEventStore" />.
/// </summary>
public class EventStore : IEventStore
{
    readonly ICommitEvents _events;
    readonly ICommitAggregateEvents _aggregateEvents;
    readonly IFetchEventsForAggregate _eventsForAggregate;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStore"/> class.
    /// </summary>
    /// <param name="events">The <see cref="ICommitEvents" />.</param>
    /// <param name="aggregateEvents">The <see cref="ICommitAggregateEvents" />.</param>
    /// <param name="eventsForAggregate">The <see cref="IFetchEventsForAggregate" />.</param>
    public EventStore(
        ICommitEvents events,
        ICommitAggregateEvents aggregateEvents,
        IFetchEventsForAggregate eventsForAggregate)
    {
        _events = events;
        _aggregateEvents = aggregateEvents;
        _eventsForAggregate = eventsForAggregate;
    }

    /// <inheritdoc/>
    public Task<CommittedEvents> CommitEvent(
        object content,
        EventSourceId eventSourceId,
        CancellationToken cancellationToken = default)
        => _events.CommitEvent(content, eventSourceId, cancellationToken);

    /// <inheritdoc/>
    public Task<CommittedEvents> Commit(
        Action<UncommittedEventsBuilder> callback,
        CancellationToken cancellationToken = default)
        => _events.Commit(callback, cancellationToken);

    /// <inheritdoc/>
    public CommitForAggregateBuilder ForAggregate(
        AggregateRootId aggregateRootId,
        CancellationToken cancellationToken = default)
        => _aggregateEvents.ForAggregate(
            aggregateRootId,
            cancellationToken);

    /// <inheritdoc/>
    public Task<CommittedEvents> CommitPublicEvent(
        object content,
        EventSourceId eventSourceId,
        CancellationToken cancellationToken = default)
        => _events.CommitPublicEvent(content, eventSourceId, cancellationToken);

    /// <inheritdoc/>
    public Task<CommittedAggregateEvents> FetchForAggregate(
        AggregateRootId aggregateRootId,
        EventSourceId eventSourceId,
        CancellationToken cancellationToken = default)
        => _eventsForAggregate.FetchForAggregate(aggregateRootId, eventSourceId, cancellationToken);
    
    /// <inheritdoc/>
    public Task<CommittedAggregateEvents> FetchForAggregate(
        AggregateRootId aggregateRootId,
        EventSourceId eventSourceId,
        IEnumerable<EventType> eventTypes,
        CancellationToken cancellationToken = default)
        => _eventsForAggregate.FetchForAggregate(aggregateRootId, eventSourceId, eventTypes, cancellationToken);
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dolittle.SDK.Events.Store;

/// <summary>
/// Defines a system can that fetch <see cref="CommittedAggregateEvents" /> for aggregate.
/// </summary>
public interface IFetchEventsForAggregate
{
    /// <summary>
    /// Fetches all the <see cref="CommittedAggregateEvents" /> for an aggregate root.
    /// </summary>
    /// <param name="aggregateRootId">The <see cref="AggregateRootId" /> of the aggregate root.</param>
    /// <param name="eventSourceId">The <see cref="EventSourceId" />.</param>
    /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
    /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="CommittedAggregateEvents" />.</returns>
    Task<CommittedAggregateEvents> FetchForAggregate(
        AggregateRootId aggregateRootId,
        EventSourceId eventSourceId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches a subset of the <see cref="CommittedAggregateEvents" /> for an aggregate root based on the <see cref="EventType"/>.
    /// </summary>
    /// <param name="aggregateRootId">The <see cref="AggregateRootId" /> of the aggregate root.</param>
    /// <param name="eventSourceId">The <see cref="EventSourceId"/>.</param>
    /// <param name="eventTypes">The <see cref="IEnumerable{T}"/> of <see cref="EventType"/> of the events to fetch.</param>
    /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
    /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="CommittedAggregateEvents" />.</returns>
    Task<CommittedAggregateEvents> FetchForAggregate(
        AggregateRootId aggregateRootId,
        EventSourceId eventSourceId,
        IEnumerable<EventType> eventTypes,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Fetches all the <see cref="CommittedAggregateEvents" /> for an aggregate root.
    /// </summary>
    /// <param name="aggregateRootId">The <see cref="AggregateRootId" /> of the aggregate root.</param>
    /// <param name="eventSourceId">The <see cref="EventSourceId" />.</param>
    /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
    /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="CommittedAggregateEvents" />.</returns>
    IAsyncEnumerable<CommittedAggregateEvents> FetchStreamForAggregate(
        AggregateRootId aggregateRootId,
        EventSourceId eventSourceId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches a subset of the <see cref="CommittedAggregateEvents" /> for an aggregate root based on the <see cref="EventType"/>.
    /// </summary>
    /// <param name="aggregateRootId">The <see cref="AggregateRootId" /> of the aggregate root.</param>
    /// <param name="eventSourceId">The <see cref="EventSourceId"/>.</param>
    /// <param name="eventTypes">The <see cref="IEnumerable{T}"/> of <see cref="EventType"/> of the events to fetch.</param>
    /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
    /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="CommittedAggregateEvents" />.</returns>
    IAsyncEnumerable<CommittedAggregateEvents> FetchStreamForAggregate(
        AggregateRootId aggregateRootId,
        EventSourceId eventSourceId,
        IEnumerable<EventType> eventTypes,
        CancellationToken cancellationToken = default);
}

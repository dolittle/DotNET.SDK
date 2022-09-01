// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
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
    /// Fetches the <see cref="CommittedAggregateEvents" /> for an aggregate root.
    /// </summary>
    /// <param name="aggregateRootId">The <see cref="AggregateRootId" /> of the aggregate root.</param>
    /// <param name="eventSourceId">The <see cref="EventSourceId" />.</param>
    /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
    /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="CommittedAggregateEvents" />.</returns>
    [Obsolete($"This method is superseded by FetchForAggregate that takes in a collection of event types used to specify which aggregate events to get.")]
    Task<CommittedAggregateEvents> FetchForAggregate(
        AggregateRootId aggregateRootId,
        EventSourceId eventSourceId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches the <see cref="CommittedAggregateEvents" /> for an aggregate root.
    /// </summary>
    /// <param name="aggregateRootId">The <see cref="AggregateRootId" /> of the aggregate root.</param>
    /// <param name="eventSourceId">The <see cref="EventSourceId"/>.</param>
    /// <param name="eventTypes">
    /// The <see cref="IEnumerable{T}"/> of <see cref="EventType"/> of the events to fetch.
    /// If none event types are given then it will get all the committed aggregate events for the aggregate root.
    /// </param>
    /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
    /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="CommittedAggregateEvents" />.</returns>
    Task<CommittedAggregateEvents> FetchForAggregate(
        AggregateRootId aggregateRootId,
        EventSourceId eventSourceId,
        IEnumerable<EventType> eventTypes,
        CancellationToken cancellationToken = default);
}

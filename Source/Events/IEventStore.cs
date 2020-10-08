// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Events.Builders;

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Defines an interface for working directly with the Event Store.
    /// </summary>
    public interface IEventStore
    {
        /// <summary>
        /// Commits an <see cref="UncommittedEvent" />.
        /// </summary>
        /// <param name="callback">The callback for creating the events to commit.</param>
        /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
        /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="CommitEventsResult" />.</returns>
        Task<CommitEventsResult> Commit(Action<UncommittedEventsBuilder> callback, CancellationToken cancellationToken = default);

        /// <summary>
        /// Commits a single Event for an aggregate with the given content.
        /// </summary>
        /// <param name="aggregateRootId">The <see cref="AggregateRootId"/> of the aggregate that applied the events to the Event Source.</param>
        /// <param name="expectedVersion">The expected <see cref="AggregateRootVersion" />.</param>
        /// <param name="eventSourceId">The <see cref="EventSourceId" />.</param>
        /// <param name="callback">The callback to create the uncommitted aggregate events.</param>
        /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
        /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="CommittedEvent" />.</returns>
        Task<CommitEventsForAggregateResult> CommitForAggregate(
            AggregateRootId aggregateRootId,
            AggregateRootVersion expectedVersion,
            EventSourceId eventSourceId,
            Action<UncommittedAggregateEventsBuilder> callback,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Fetches the <see cref="CommittedAggregateEvents" /> for an aggregate root.
        /// </summary>
        /// <param name="aggregateRootId">The <see cref="AggregateRootId" /> of the aggreaget root.</param>
        /// <param name="eventSourceId">The <see cref="EventSourceId" />.</param>
        /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
        /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="FetchForAggregateResult" />.</returns>
        Task<FetchForAggregateResult> FetchForAggregate(
            AggregateRootId aggregateRootId,
            EventSourceId eventSourceId,
            CancellationToken cancellationToken = default);
    }
}

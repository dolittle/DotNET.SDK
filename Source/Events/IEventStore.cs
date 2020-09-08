// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Defines an interface for working directly with the Event Store.
    /// </summary>
    public interface IEventStore
    {
        /// <summary>
        /// Commits <see cref="UncommittedEvents" />.
        /// </summary>
        /// <param name="uncommittedEvents">The <see cref="UncommittedEvents" />.</param>
        /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
        /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="CommittedEvents" />.</returns>
        /// <remarks>
        /// Cancelling this operation does not roll back the commit transaction if the events have already been written to the Event Store.
        /// </remarks>
        Task<CommittedEvents> Commit(UncommittedEvents uncommittedEvents, CancellationToken cancellationToken = default);

        /// <summary>
        /// Commits <see cref="UncommittedAggregateEvents" />.
        /// </summary>
        /// <param name="uncommittedAggregateEvents">The <see cref="UncommittedAggregateEvents" />.</param>
        /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
        /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="CommittedAggregateEvents" />.</returns>
        /// <remarks>
        /// Cancelling this operation does not roll back the commit transaction if the events have already been written to the Event Store.
        /// </remarks>
        Task<CommittedAggregateEvents> CommitForAggregate(UncommittedAggregateEvents uncommittedAggregateEvents, CancellationToken cancellationToken = default);

        /// <summary>
        /// Fetch <see cref="CommittedAggregateEvents" /> for a <see cref="AggregateRoot" />.
        /// </summary>
        /// <param name="aggregateRoot">The <see cref="Type"/> of the Aggregate Root that applied the events to the Event Source.</param>
        /// <param name="eventSource">The <see cref="EventSourceId" /> of the Aggregate.</param>
        /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
        /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="CommittedAggregateEvents" /> on from this Aggregate.</returns>
        Task<CommittedAggregateEvents> FetchForAggregate(Type aggregateRoot, EventSourceId eventSource, CancellationToken cancellationToken = default);

        /// <summary>
        /// Fetch <see cref="CommittedAggregateEvents" /> for a <see cref="AggregateRoot" />.
        /// </summary>
        /// <param name="eventSource">The <see cref="EventSourceId" /> of the Aggregate.</param>
        /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
        /// <typeparam name="TAggregateRoot">Thetype of the Aggregate Root that applied the events to the Event Source.</typeparam>
        /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="CommittedAggregateEvents" /> on from this Aggregate.</returns>
        Task<CommittedAggregateEvents> FetchForAggregate<TAggregateRoot>(EventSourceId eventSource, CancellationToken cancellationToken = default)
            where TAggregateRoot : AggregateRoot;
    }
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;

namespace Dolittle.SDK.Events.Store
{
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
        /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="FetchForAggregateResult" />.</returns>
        Task<FetchForAggregateResult> FetchForAggregate(
            AggregateRootId aggregateRootId,
            EventSourceId eventSourceId,
            CancellationToken cancellationToken = default);
    }
}

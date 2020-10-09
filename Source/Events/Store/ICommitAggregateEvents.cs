// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Events.Builders;

namespace Dolittle.SDK.Events.Store
{
    /// <summary>
    /// Defines a system that can commit <see cref="UncommittedAggregateEvents" />.
    /// </summary>
    public interface ICommitAggregateEvents
    {
        /// <summary>
        /// Commits a single Event for an aggregate with the given content.
        /// </summary>
        /// <param name="aggregateRootId">The <see cref="AggregateRootId"/> of the aggregate that applied the events to the Event Source.</param>
        /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
        /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="CommittedEvent" />.</returns>
        CommitForAggregateBuilder ForAggregate(
            AggregateRootId aggregateRootId,
            CancellationToken cancellationToken = default);
    }
}

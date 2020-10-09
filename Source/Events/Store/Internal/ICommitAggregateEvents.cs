// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;

namespace Dolittle.SDK.Events.Store.Internal
{
    /// <summary>
    /// Defines a system that can commit <see cref="UncommittedAggregateEvents" /> for internal use.
    /// </summary>
    public interface ICommitAggregateEvents
    {
        /// <summary>
        /// Commits a single Event for an aggregate with the given content.
        /// </summary>
        /// <param name="uncommittedAggregateEvents">The <see cref="UncommittedAggregateEvents" /> to commit.</param>
        /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
        /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="CommittedEvent" />.</returns>
        Task<CommittedAggregateEvents> CommitForAggregate(
            UncommittedAggregateEvents uncommittedAggregateEvents,
            CancellationToken cancellationToken = default);
    }
}

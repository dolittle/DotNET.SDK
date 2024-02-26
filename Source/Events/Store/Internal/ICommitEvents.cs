// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;

namespace Dolittle.SDK.Events.Store.Internal;

/// <summary>
/// Defines a system that can commit <see cref="UncommittedEvents" /> for internal use.
/// </summary>
public interface ICommitEvents
{
    /// <summary>
    /// Commits uncommitted events.
    /// </summary>
    /// <param name="uncommittedEvents">The <see cref="UncommittedEvents" /> to commit.</param>
    /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
    /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="CommittedEvents" />.</returns>
    Task<CommittedEvents> Commit(
        UncommittedEvents uncommittedEvents,
        CancellationToken cancellationToken = default);
}

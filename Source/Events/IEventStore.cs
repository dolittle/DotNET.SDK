// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
        /// Commits a single Event with the given content.
        /// </summary>
        /// <param name="content">The content of the Event.</param>
        /// <param name="eventSourceId">The <see cref="EventSourceId" />.</param>
        /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
        /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="CommittedEvent" />.</returns>
        Task<CommitEventsResult> Commit(object content, EventSourceId eventSourceId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Commits a single Event with the given content.
        /// </summary>
        /// <param name="content">The content of the Event.</param>
        /// <param name="eventSourceId">The <see cref="EventSourceId" />.</param>
        /// <param name="eventType">The <see cref="EventType"/> the Event is associated with.</param>
        /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
        /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="CommittedEvent" />.</returns>
        Task<CommitEventsResult> Commit(object content, EventSourceId eventSourceId, EventType eventType, CancellationToken cancellationToken = default);

        /// <summary>
        /// Commits a single Event with the given content.
        /// </summary>
        /// <param name="content">The content of the Event.</param>
        /// <param name="eventSourceId">The <see cref="EventSourceId" />.</param>
        /// <param name="eventTypeId">The <see cref="EventTypeId"/> the Event is associated with.</param>
        /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
        /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="CommittedEvent" />.</returns>
        Task<CommitEventsResult> Commit(object content, EventSourceId eventSourceId, EventTypeId eventTypeId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Commits a single public Event with the given content.
        /// </summary>
        /// <param name="content">The content of the Event.</param>
        /// <param name="eventSourceId">The <see cref="EventSourceId" />.</param>
        /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
        /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="CommittedEvent" />.</returns>
        Task<CommitEventsResult> CommitPublic(object content, EventSourceId eventSourceId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Commits a single public Event with the given content.
        /// </summary>
        /// <param name="content">The content of the Event.</param>
        /// <param name="eventSourceId">The <see cref="EventSourceId" />.</param>
        /// <param name="eventType">The <see cref="EventType"/> the Event is associated with.</param>
        /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
        /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="CommittedEvent" />.</returns>
        Task<CommitEventsResult> CommitPublic(object content, EventSourceId eventSourceId, EventType eventType, CancellationToken cancellationToken = default);

        /// <summary>
        /// Commits a single public Event with the given content.
        /// </summary>
        /// <param name="content">The content of the Event.</param>
        /// <param name="eventSourceId">The <see cref="EventSourceId" />.</param>
        /// <param name="eventTypeId">The <see cref="EventTypeId"/> the Event is associated with.</param>
        /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
        /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="CommittedEvent" />.</returns>
        Task<CommitEventsResult> CommitPublic(object content, EventSourceId eventSourceId, EventTypeId eventTypeId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Commits an <see cref="UncommittedEvent" />.
        /// </summary>
        /// <param name="uncommittedEvent">The <see cref="UncommittedEvent" />.</param>
        /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
        /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="CommittedEvent" />.</returns>
        Task<CommitEventsResult> Commit(UncommittedEvent uncommittedEvent, CancellationToken cancellationToken = default);

        /// <summary>
        /// Commits <see cref="UncommittedEvents" />.
        /// </summary>
        /// <param name="uncommittedEvents">The <see cref="UncommittedEvents" />.</param>
        /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
        /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="CommittedEvents" />.</returns>
        /// <remarks>
        /// Cancelling this operation does not roll back the commit transaction if the events have already been written to the Event Store.
        /// </remarks>
        Task<CommitEventsResult> Commit(UncommittedEvents uncommittedEvents, CancellationToken cancellationToken = default);
    }
}

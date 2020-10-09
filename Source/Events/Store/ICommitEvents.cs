// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Events.Builders;

namespace Dolittle.SDK.Events.Store
{
    /// <summary>
    /// Defines a system that can commit <see cref="UncommittedEvents" />.
    /// </summary>
    public interface ICommitEvents
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
        /// <param name="callback">The callback for creating the events to commit.</param>
        /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
        /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="CommitEventsResult" />.</returns>
        Task<CommitEventsResult> Commit(Action<UncommittedEventsBuilder> callback, CancellationToken cancellationToken = default);
    }
}

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

        /// <summary>
        /// Commits a single Event for an aggregate with the given content.
        /// </summary>
        /// <param name="content">The content of the Event.</param>
        /// <param name="eventSourceId">The <see cref="EventSourceId" />.</param>
        /// <param name="aggregateRootId">The <see cref="AggregateRootId"/> of the aggregate that applied the events to the Event Source.</param>
        /// <param name="expectedAggregateRootVersion">The <see cref="AggregateRootVersion"/> of the Aggregate Root that was used to apply the rules that resulted in the Events.</param>
        /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
        /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="CommittedEvent" />.</returns>
        Task<CommitEventsForAggregateResult> CommitForAggregate(
            object content,
            EventSourceId eventSourceId,
            AggregateRootId aggregateRootId,
            AggregateRootVersion expectedAggregateRootVersion,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Commits a single Event for an aggregate with the given content.
        /// </summary>
        /// <param name="content">The content of the Event.</param>
        /// <param name="eventSourceId">The <see cref="EventSourceId" />.</param>
        /// <param name="aggregateRootId">The <see cref="AggregateRootId"/> of the aggregate that applied the events to the Event Source.</param>
        /// <param name="expectedAggregateRootVersion">The <see cref="AggregateRootVersion"/> of the Aggregate Root that was used to apply the rules that resulted in the Events.</param>
        /// <param name="eventType">The <see cref="EventType"/> the Event is associated with.</param>
        /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
        /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="CommittedEvent" />.</returns>
        Task<CommitEventsForAggregateResult> CommitForAggregate(
            object content,
            EventSourceId eventSourceId,
            AggregateRootId aggregateRootId,
            AggregateRootVersion expectedAggregateRootVersion,
            EventType eventType,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Commits a single public event for an aggregate with the given content.
        /// </summary>
        /// <param name="content">The content of the Event.</param>
        /// <param name="eventSourceId">The <see cref="EventSourceId" />.</param>
        /// <param name="aggregateRootId">The <see cref="AggregateRootId"/> of the aggregate that applied the events to the Event Source.</param>
        /// <param name="expectedAggregateRootVersion">The <see cref="AggregateRootVersion"/> of the Aggregate Root that was used to apply the rules that resulted in the Events.</param>
        /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
        /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="CommittedEvent" />.</returns>
        Task<CommitEventsForAggregateResult> CommitPublicForAggregate(
            object content,
            EventSourceId eventSourceId,
            AggregateRootId aggregateRootId,
            AggregateRootVersion expectedAggregateRootVersion,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Commits a single public event for an aggregate with the given content.
        /// </summary>
        /// <param name="content">The content of the Event.</param>
        /// <param name="eventSourceId">The <see cref="EventSourceId" />.</param>
        /// <param name="aggregateRootId">The <see cref="AggregateRootId"/> of the aggregate that applied the events to the Event Source.</param>
        /// <param name="expectedAggregateRootVersion">The <see cref="AggregateRootVersion"/> of the Aggregate Root that was used to apply the rules that resulted in the Events.</param>
        /// <param name="eventType">The <see cref="EventType"/> the Event is associated with.</param>
        /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
        /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="CommittedEvent" />.</returns>
        Task<CommitEventsForAggregateResult> CommitPublicForAggregate(
            object content,
            EventSourceId eventSourceId,
            AggregateRootId aggregateRootId,
            AggregateRootVersion expectedAggregateRootVersion,
            EventType eventType,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Commits a single public Event for an aggregate with the given content.
        /// </summary>
        /// <param name="content">The content of the Event.</param>
        /// <param name="eventSourceId">The <see cref="EventSourceId" />.</param>
        /// <param name="aggregateRootId">The <see cref="AggregateRootId"/> of the aggregate that applied the events to the Event Source.</param>
        /// <param name="expectedAggregateRootVersion">The <see cref="AggregateRootVersion"/> of the Aggregate Root that was used to apply the rules that resulted in the Events.</param>
        /// <param name="eventTypeId">The <see cref="EventTypeId"/> the Event is associated with.</param>
        /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
        /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="CommittedEvent" />.</returns>
        Task<CommitEventsForAggregateResult> CommitPublicForAggregate(
            object content,
            EventSourceId eventSourceId,
            AggregateRootId aggregateRootId,
            AggregateRootVersion expectedAggregateRootVersion,
            EventTypeId eventTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Commits a single Event for an aggregate with the given content.
        /// </summary>
        /// <param name="content">The content of the Event.</param>
        /// <param name="eventSourceId">The <see cref="EventSourceId" />.</param>
        /// <param name="aggregateRootId">The <see cref="AggregateRootId"/> of the aggregate that applied the events to the Event Source.</param>
        /// <param name="expectedAggregateRootVersion">The <see cref="AggregateRootVersion"/> of the Aggregate Root that was used to apply the rules that resulted in the Events.</param>
        /// <param name="eventTypeId">The <see cref="EventTypeId"/> the Event is associated with.</param>
        /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
        /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="CommittedEvent" />.</returns>
        Task<CommitEventsForAggregateResult> CommitForAggregate(
            object content,
            EventSourceId eventSourceId,
            AggregateRootId aggregateRootId,
            AggregateRootVersion expectedAggregateRootVersion,
            EventTypeId eventTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Commits the <see cref="UncommittedAggregateEvent"/>.
        /// </summary>
        /// <param name="uncommittedAggregateEvent">The <see cref="UncommittedAggregateEvent"/> to commit.</param>
        /// <param name="eventSourceId">The <see cref="EventSourceId" />.</param>
        /// <param name="aggregateRootId">The <see cref="AggregateRootId"/> of the aggregate that applied the events to the Event Source.</param>
        /// <param name="expectedAggregateRootVersion">The <see cref="AggregateRootVersion"/> of the Aggregate Root that was used to apply the rules that resulted in the Events.</param>
        /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
        /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="CommitEventsForAggregateResult" />.</returns>
        Task<CommitEventsForAggregateResult> CommitForAggregate(
            UncommittedAggregateEvent uncommittedAggregateEvent,
            EventSourceId eventSourceId,
            AggregateRootId aggregateRootId,
            AggregateRootVersion expectedAggregateRootVersion,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Commits the <see cref="UncommittedAggregateEvents"/>.
        /// </summary>
        /// <param name="uncommittedAggregateEvents">The <see cref="UncommittedAggregateEvents"/> to commit.</param>
        /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
        /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="CommitEventsForAggregateResult" />.</returns>
        Task<CommitEventsForAggregateResult> CommitForAggregate(
            UncommittedAggregateEvents uncommittedAggregateEvents,
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

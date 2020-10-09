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
    public partial interface ICommitAggregateEvents
    {
        /// <summary>
        /// Commits a single Event for an aggregate with the given content.
        /// </summary>
        /// <param name="aggregateRootId">The <see cref="AggregateRootId"/> of the aggregate that applied the events to the Event Source.</param>
        /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
        /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="CommittedEvent" />.</returns>
        CommitForAggregateBuilder CommitForAggregate(
            AggregateRootId aggregateRootId,
            CancellationToken cancellationToken = default);

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
    }
}

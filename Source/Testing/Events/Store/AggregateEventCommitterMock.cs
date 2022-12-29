// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Store;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;
using ICommitAggregateEvents = Dolittle.SDK.Events.Store.Internal.ICommitAggregateEvents;

namespace Dolittle.SDK.Testing.Events.Store
{
    /// <summary>
    /// Represents a mock of <see cref="ICommitAggregateEvents"/> that returns committed events as if the event log was empty.
    /// </summary>
    /// <remarks>
    /// The <see cref="Execution.ExecutionContext"/> of the <see cref="CommittedEvents"/> is empty.
    /// The implementation is not thread-safe.
    /// </remarks>
    public class AggregateEventCommitterMock : ICommitAggregateEvents
    {
        readonly EventCommitterMock _committerMock;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateEventCommitterMock"/> class.
        /// </summary>
        public AggregateEventCommitterMock()
        {
            _committerMock = new EventCommitterMock();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateEventCommitterMock"/> class.
        /// </summary>
        /// <param name="eventCommitterMock">An event committer mock to synchronise events and sequence number with.</param>
        public AggregateEventCommitterMock(EventCommitterMock eventCommitterMock)
        {
            _committerMock = eventCommitterMock;
        }

        /// <summary>
        /// Gets the empty <see cref="ExecutionContext"/> used for the mock.
        /// </summary>
        public ExecutionContext ExecutionContext => _committerMock.ExecutionContext;

        /// <summary>
        /// Gets or sets the current <see cref="EventLogSequenceNumber"/>.
        /// </summary>
        public EventLogSequenceNumber SequenceNumber
        {
            get
            {
                return _committerMock.SequenceNumber;
            }

            set
            {
                _committerMock.SequenceNumber = value;
            }
        }

        /// <summary>
        /// Gets all the <see cref="CommittedEvent"/> that has been committed on this mock.
        /// </summary>
        public List<CommittedEvent> CommittedEvents => _committerMock.CommittedEvents;

        /// <inheritdoc />
        public Task<CommittedAggregateEvents> CommitForAggregate(UncommittedAggregateEvents uncommittedAggregateEvents, CancellationToken cancellationToken = default)
        {
            var events = new List<CommittedAggregateEvent>();
            var aggregateRootVersion = uncommittedAggregateEvents.ExpectedAggregateRootVersion;

            foreach (var uncommittedEvent in uncommittedAggregateEvents)
            {
                events.Add(new CommittedAggregateEvent(
                    SequenceNumber,
                    DateTimeOffset.Now,
                    uncommittedAggregateEvents.EventSource,
                    uncommittedAggregateEvents.AggregateRoot,
                    aggregateRootVersion,
                    ExecutionContext,
                    uncommittedEvent.EventType,
                    uncommittedEvent.Content,
                    uncommittedEvent.IsPublic));
                aggregateRootVersion++;
                SequenceNumber++;
            }

            CommittedEvents.AddRange(events);

            return Task.FromResult(new CommittedAggregateEvents(
                uncommittedAggregateEvents.EventSource,
                uncommittedAggregateEvents.AggregateRoot,
                events));
        }
    }
}
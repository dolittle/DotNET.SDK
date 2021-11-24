// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Store;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Microservices;
using Dolittle.SDK.Security;
using Dolittle.SDK.Tenancy;
using Environment = Dolittle.SDK.Microservices.Environment;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;
using Version = Dolittle.SDK.Microservices.Version;

namespace Dolittle.SDK.Testing.Events.Store
{
    /// <summary>
    /// Represents a mock of <see cref="IFetchEventsForAggregate"/> that returns committed events as if the event log was empty.
    /// </summary>
    /// <remarks>
    /// The <see cref="ExecutionContext"/> of the <see cref="CommittedEvents"/> is empty.
    /// The same set of events will be returned for all calls to <see cref="FetchForAggregate"/> regardless of arguments.
    /// </remarks>
    public class AggregateEventFetcherMock : IFetchEventsForAggregate
    {
        readonly IEventTypes _eventTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateEventFetcherMock"/> class.
        /// </summary>
        /// <param name="eventTypes">The event types that will be used to resolve <see cref="EventType"/> for the <see cref="CommittedEvents"/>.</param>
        public AggregateEventFetcherMock(IEventTypes eventTypes)
        {
            _eventTypes = eventTypes;
        }

        /// <summary>
        /// Gets the empty <see cref="ExecutionContext"/> used for the mock.
        /// </summary>
        public ExecutionContext ExecutionContext { get; } = new ExecutionContext(
            MicroserviceId.NotSet,
            TenantId.Unknown,
            Version.NotSet,
            Environment.Undetermined,
            CorrelationId.System,
            Claims.Empty,
            CultureInfo.CurrentCulture);

        /// <summary>
        /// Gets the committed events that will be returned when <see cref="FetchForAggregate"/> is called.
        /// </summary>
        public List<object> CommittedEvents { get; } = new List<object>();

        /// <inheritdoc />
        public Task<CommittedAggregateEvents> FetchForAggregate(AggregateRootId aggregateRootId, EventSourceId eventSourceId, CancellationToken cancellationToken = default)
        {
            var events = new List<CommittedAggregateEvent>();

            var sequenceNumber = EventLogSequenceNumber.Initial;
            var aggregateVersion = AggregateRootVersion.Initial;

            foreach (var committedEvent in CommittedEvents)
            {
                events.Add(new CommittedAggregateEvent(
                    sequenceNumber,
                    DateTimeOffset.Now,
                    eventSourceId,
                    aggregateRootId,
                    aggregateVersion,
                    ExecutionContext,
                    _eventTypes.GetFor(committedEvent.GetType()),
                    committedEvent,
                    false));
                aggregateVersion++;
                sequenceNumber++;
            }

            return Task.FromResult(new CommittedAggregateEvents(
                eventSourceId,
                aggregateRootId,
                events));
        }
    }
}
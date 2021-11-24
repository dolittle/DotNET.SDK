// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Store;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Dolittle.SDK.Testing.Events.Store
{
    /// <summary>
    /// Represents a mock of <see cref="EventStore"/> that uses the actual implementation, but allows to prefill the
    /// event log to use for fetching for an aggregate, and keeps track of the committed events.
    /// </summary>
    public class EventStoreMock : EventStore
    {
        readonly AggregateEventCommitterMock _committerMock;
        readonly AggregateEventFetcherMock _fetcherMock;

        EventStoreMock(EventCommitterMock committerMock, AggregateEventCommitterMock aggregateCommitterMock, AggregateEventFetcherMock fetcherMock, IEventTypes eventTypes, ILogger logger)
            : base(new EventCommitter(committerMock, eventTypes), new AggregateEventCommitter(aggregateCommitterMock, eventTypes, logger), fetcherMock)
        {
            _committerMock = aggregateCommitterMock;
            _fetcherMock = fetcherMock;
        }

        /// <summary>
        /// Gets all the <see cref="CommittedEvent"/> that has been committed on this mock.
        /// </summary>
        public List<CommittedEvent> CommittedEvents => _committerMock.CommittedEvents;

        /// <summary>
        /// Gets the committed events that will be returned when fetching aggregate events.
        /// </summary>
        public List<object> EventsToFetch => _fetcherMock.CommittedEvents;

        /// <summary>
        /// Creates a new instance of the <see cref="EventStoreMock"/>.
        /// </summary>
        /// <param name="eventTypes">The <see cref="IEventTypes"/> to use.</param>
        /// <returns>The newly created <see cref="EventStoreMock"/>.</returns>
        public static EventStoreMock Create(IEventTypes eventTypes)
        {
            var logger = NullLogger.Instance;
            var committerMock = new EventCommitterMock();
            var aggregateCommitterMock = new AggregateEventCommitterMock(committerMock);
            var fetcherMock = new AggregateEventFetcherMock(eventTypes);

            return new EventStoreMock(
                committerMock,
                aggregateCommitterMock,
                fetcherMock,
                eventTypes,
                logger);
        }
    }
}
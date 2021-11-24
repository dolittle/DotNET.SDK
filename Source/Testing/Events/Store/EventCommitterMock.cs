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
using ICommitEvents = Dolittle.SDK.Events.Store.Internal.ICommitEvents;
using Version = Dolittle.SDK.Microservices.Version;

namespace Dolittle.SDK.Testing.Events.Store
{
    /// <summary>
    /// Represents a mock of <see cref="ICommitEvents"/> that returns committed events as if the event log was empty.
    /// </summary>
    /// <remarks>
    /// The <see cref="ExecutionContext"/> of the <see cref="CommittedEvents"/> is empty.
    /// The implementation is not thread-safe.
    /// </remarks>
    public class EventCommitterMock : ICommitEvents
    {
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
        /// Gets or sets the current <see cref="EventLogSequenceNumber"/>.
        /// </summary>
        public EventLogSequenceNumber SequenceNumber { get; set; } = EventLogSequenceNumber.Initial;

        /// <summary>
        /// Gets all the <see cref="CommittedEvent"/> that has been committed on this mock.
        /// </summary>
        public List<CommittedEvent> CommittedEvents { get; } = new List<CommittedEvent>();

        /// <inheritdoc />
        public Task<CommittedEvents> Commit(UncommittedEvents uncommittedEvents, CancellationToken cancellationToken = default)
        {
            var events = new List<CommittedEvent>();

            foreach (var uncommittedEvent in uncommittedEvents)
            {
                events.Add(new CommittedEvent(
                    SequenceNumber,
                    DateTimeOffset.Now,
                    uncommittedEvent.EventSource,
                    ExecutionContext,
                    uncommittedEvent.EventType,
                    uncommittedEvent.Content,
                    uncommittedEvent.IsPublic));
                SequenceNumber++;
            }

            CommittedEvents.AddRange(events);

            return Task.FromResult(new CommittedEvents(events));
        }
    }
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.ApplicationModel;
using Dolittle.Events;
using Dolittle.Execution;
using Dolittle.Tenancy;

namespace Dolittle.Domain.for_AggregateRoot.given
{
    public abstract class committed_events_and_two_aggregate_roots : two_aggregate_roots
    {
        static CorrelationId correlationId = Guid.Parse("2105cf5d-3134-41c3-9617-5a9e8c833ca4");
        static Microservice microserviceId = Guid.Parse("9624658b-8caf-44b8-b891-7f53f69f8b5e");
        static TenantId tenantId = Guid.Parse("54ad514b-baa5-44f5-8a6b-870d2ce0dcb2");
        static Cause cause = new Cause(CauseType.Command, 0);

        public static CommittedAggregateEvents build_committed_events(EventSourceId eventSource, Type aggregateRoot, ExecutionContext executionContext)
        {
            var events = new List<CommittedAggregateEvent>()
            {
                build_committed_event(eventSource, aggregateRoot, 0, 0, event_one, executionContext),
                build_committed_event(eventSource, aggregateRoot, 1, 1, event_two, executionContext),
                build_committed_event(eventSource, aggregateRoot, 2, 2, event_three, executionContext),
            };
            return new CommittedAggregateEvents(eventSource, aggregateRoot, events);
        }

        static CommittedAggregateEvent build_committed_event(EventSourceId eventSource, Type aggregateRoot, AggregateRootVersion aggregateRootVersion, EventLogSequenceNumber eventLogSequenceNumber, IEvent @event, ExecutionContext executionContext) =>
            new CommittedAggregateEvent(eventLogSequenceNumber, DateTimeOffset.Now, eventSource, aggregateRoot, aggregateRootVersion, executionContext, @event);
    }
}

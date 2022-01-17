// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Store;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Security;
using Machine.Specifications;
using Version = Dolittle.SDK.Microservices.Version;

namespace Dolittle.SDK.Aggregates.for_AggregateRoot.given;

public abstract class committed_events_and_two_aggregate_roots : two_aggregate_roots
{
    protected static ExecutionContext execution_context;

    Establish context = () =>
    {
        execution_context = new ExecutionContext(
            "cb4648a8-6a4a-4f2b-a095-e46b288a3a4e",
            "7f6db119-f58e-46eb-b5ec-2b85e1b8ab01",
            Version.NotSet,
            "environment",
            "d3f22054-f487-4ce6-8831-3283d881e282",
            Claims.Empty,
            CultureInfo.InvariantCulture);
    };

    public static CommittedAggregateEvents build_committed_events(EventSourceId eventSource, AggregateRootId aggregateRootId, ExecutionContext executionContext)
    {
        var events = new List<CommittedAggregateEvent>()
        {
            build_committed_event(
                eventSource,
                aggregateRootId,
                0,
                0,
                first_event,
                event_types.GetFor(first_event.GetType()),
                false,
                executionContext),
            build_committed_event(
                eventSource,
                aggregateRootId,
                1,
                1,
                second_event,
                event_types.GetFor(second_event.GetType()),
                false,
                executionContext),
            build_committed_event(
                eventSource,
                aggregateRootId,
                2,
                2,
                third_event,
                event_types.GetFor(third_event.GetType()),
                false,
                executionContext),
        };
        return new CommittedAggregateEvents(eventSource, aggregateRootId, events);
    }

    static CommittedAggregateEvent build_committed_event(
        EventSourceId eventSource,
        AggregateRootId aggregateRootId,
        AggregateRootVersion aggregateRootVersion,
        EventLogSequenceNumber eventLogSequenceNumber,
        object @event,
        EventType eventType,
        bool isPublic,
        ExecutionContext executionContext)
        => new(
            eventLogSequenceNumber,
            DateTimeOffset.Now,
            eventSource,
            aggregateRootId,
            aggregateRootVersion,
            executionContext,
            eventType,
            @event,
            isPublic);
}
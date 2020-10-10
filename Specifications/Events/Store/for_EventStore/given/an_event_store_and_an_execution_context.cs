// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.SDK.Events.Store.Converters;
using Dolittle.SDK.Services;
using Machine.Specifications;
using Moq;
using Contracts = Dolittle.Runtime.Events.Contracts;

namespace Dolittle.SDK.Events.Store.for_EventStore.given
{
    public class an_event_store_and_an_execution_context : an_execution_context
    {
        protected static Mock<IPerformMethodCalls> caller;
        protected static Mock<IConvertEventsToProtobuf> events_to_protobuf_converter;
        protected static Mock<IConvertAggregateEventsToProtobuf> aggregate_events_to_protobuf_converter;
        protected static Mock<IConvertEventResponsesToSDK> events_to_sdk_converter;
        protected static Mock<IConvertAggregateResponsesToSDK> aggregate_to_sdk_converter;
        protected static Mock<IEventTypes> event_types;
        protected static Mock<IResolveCallContext> call_context_resolver;
        protected static IEventStore event_store;
        protected static Contracts.CommitEventsRequest commit_events_request;
        protected static Contracts.CommitEventsResponse commit_events_response;
        protected static IEnumerable<Contracts.UncommittedEvent> pb_uncommitted_events;

        Establish context = () =>
        {
            // caller = new Mock<IPerformMethodCalls>();
            // event_types = new Mock<IEventTypes>();
            // converter = new Mock<IEventConverter>();
            // events_to_protobuf_converter = new Mock<IConvertEventsToProtobuf>();
            // aggregate_events_to_protobuf_converter = new Mock<IConvertAggregateEventsToProtobuf>();
            // events_to_sdk_converter = new Mock<IConvertEventResponsestoSDK>();
            // aggregate_to_sdk_converter = new Mock<IConvertAggregateResponsesToSDK>();
            // call_context_resolver = new Mock<IResolveCallContext>();
            // var eventCommitter = new EventCommitter(new Store.Internal.EventCommitter(caller.Object, converter.Object, call_context_resolver.Object, execution_context, Mock.Of<ILogger>()), event_types.Object);
            // var aggregateEventCommitter = new AggregateEventCommitter(new Store.Internal.AggregateEventCommitter(caller.Object, converter.Object, call_context_resolver.Object, execution_context, Mock.Of<ILogger>()), event_types.Object, Mock.Of<ILogger>());
            // var eventsForAggregateFetcher = new Store.Internal.EventsForAggregateFetcher(caller.Object, converter.Object, call_context_resolver.Object, execution_context, Mock.Of<ILogger>());
            // event_store = new EventStore(eventCommitter, aggregateEventCommitter, eventsForAggregateFetcher);

            // event_store = new EventStore(
            //     caller.Object,
            //     execution_context,
            //     event_types.Object,
            //     events_to_protobuf_converter.Object,
            //     aggregate_events_to_protobuf_converter.Object,
            //     events_to_sdk_converter.Object,
            //     aggregate_to_sdk_converter.Object,
            //     Mock.Of<ILogger>());

            // pb_uncommitted_events = new List<Contracts.UncommittedEvent>();
            // pb_uncommitted_events.Append(new Contracts.UncommittedEvent());
            // pb_uncommitted_events.Append(new Contracts.UncommittedEvent());

            // converter.Setup(_ => _.ToProtobuf(Moq.It.IsAny<UncommittedEvents>()))
            //     .Returns(pb_uncommitted_events);

            // var pb_committed_events = new List<Contracts.CommittedEvent>();
            // pb_committed_events.Append(new Contracts.CommittedEvent());
            // pb_committed_events.Append(new Contracts.CommittedEvent());

            // commit_events_response = new Contracts.CommitEventsResponse();

            // caller.Setup(_ => _.Call(Moq.It.IsAny<EventStoreCommitMethod>(), Moq.It.IsAny<Contracts.CommitEventsRequest>(), Moq.It.IsAny<CancellationToken>()))
            //     .Callback<ICanCallAUnaryMethod<Contracts.CommitEventsRequest, Contracts.CommitEventsResponse>, Contracts.CommitEventsRequest, CancellationToken>((method, request, token) => commit_events_request = request)
            //     .Returns(Task.FromResult(commit_events_response));

            // var failure = new Failure(Guid.Parse("72ae75dc-cdd7-4413-bf19-66aba13486ad"), "ran out of tacos");
            // var committedEvents = new CommittedEvents(new List<CommittedEvent>());

            // converter.Setup(_ => _.ToSDK(commit_events_response.Events))
            //     .Returns(committedEvents);
        };
    }
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.SDK.Events.for_EventConverter.given;
using Dolittle.SDK.Protobuf;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.Events.Store.for_EventStore
{
    public class when_committing_an_event : given.an_event_store_and_an_execution_context
    {
        static an_event content;
        static EventSourceId event_source;
        static EventType event_type;
        static CommittedEvents result;

        Establish context = () =>
        {
            content = new an_event("goodbye world", 12345, true);
            event_source = new EventSourceId("e4799653-eb7d-481b-9548-3156ddb45832");
            event_type = new EventType("843c431e-7cd6-43d0-a3e2-e82d6da0218b");

            event_types.Setup(_ => _.GetFor(content.GetType())).Returns(event_type);
            event_types.Setup(_ => _.HasFor(content.GetType())).Returns(true);
        };

        Because of = () => result = event_store.CommitEvent(content, event_source).Result;

        It should_call_the_event_types_with_the_content = () => event_types.Verify(_ => _.GetFor(content.GetType()));
        It should_call_the_converter_with_uncommitted_events = () => converter.Verify(_ => _.ToProtobuf(Moq.It.IsAny<UncommittedEvents>()));
        It should_have_the_same_values_on_the_uncommitted_events = () => converter.Verify(_ => _.ToProtobuf(
            Moq.It.Is<UncommittedEvents>(_ => _[0].Content == content && _[0].EventSource == event_source && _[0].EventType == event_type)));

        It should_call_the_caller_with_the_correct_request = () => caller.Verify(_ => _.Call(Moq.It.IsAny<EventStoreCommitMethod>(), commit_events_request, Moq.It.IsAny<CancellationToken>()), Times.Once());
        It should_set_the_execution_context_to_the_call_context = () => commit_events_request.CallContext.ExecutionContext.ShouldEqual(execution_context.ToProtobuf());
        It should_set_the_events_in_the_request = () => commit_events_request.Events.ShouldEqual(pb_uncommitted_events);
        It should_call_the_converter_with_results_from_the_caller = () => converter.Verify(_ => _.ToSDK(commit_events_response));
    }
}

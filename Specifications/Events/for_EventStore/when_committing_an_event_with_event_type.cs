// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.SDK.Events.for_EventConverter.given;
using Dolittle.SDK.Protobuf;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.Events.for_EventStore
{
    public class when_committing_an_event_with_event_type : given.an_event_store_and_an_execution_context
    {
        static an_event content;
        static EventSourceId event_source;
        static EventType event_type;

        Establish context = () =>
        {
            content = new an_event("goodbye world", 12345, false);
            event_source = new EventSourceId("6bb48542-29b7-4339-a711-b8c8ca733f9d");
            event_type = new EventType("b40a6161-44dd-481f-a1ff-40694c05f91d");
        };

        Because of = async () => await event_store.Commit(content, event_source, event_type);
        It should_not_call_the_event_types_with_the_content = () => event_types.Verify(_ => _.GetFor(content.GetType()), Times.Never());
        It should_call_the_converter_with_uncommitted_events = () => converter.Verify(_ => _.ToProtobuf(Moq.It.IsAny<UncommittedEvents>()));
        It should_call_the_caller_with_the_correct_request = () => caller.Verify(_ => _.Call(Moq.It.IsAny<EventStoreCommitMethod>(), commit_events_request, Moq.It.IsAny<CancellationToken>()), Times.Once());
        It should_set_the_execution_context_to_the_call_context = () => commit_events_request.CallContext.ExecutionContext.ShouldEqual(execution_context.ToProtobuf());
        It should_set_the_events_in_the_request = () => commit_events_request.Events.ShouldEqual(pb_uncommitted_events);
        It should_call_the_converter_with_results_from_the_caller = () => converter.Verify(_ => _.ToSDK(commit_events_response));
    }
}

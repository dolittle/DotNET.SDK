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
    public class when_committing_an_uncommitted_event : given.an_event_store_and_an_execution_context
    {
        static UncommittedEvent uncommitted_event;
        static an_event content;
        static EventSourceId event_source;
        static EventType event_type;
        static bool is_public;
        static CommitEventsResult result;

        Establish context = () =>
        {
            content = new an_event("goodbye world", 12345, false);
            event_source = new EventSourceId("cc82c47c-2012-4916-9452-1215c96a5550");
            event_type = new EventType("18e54777-fddb-4d48-b86d-05161c3dc288");
            is_public = false;
            uncommitted_event = new UncommittedEvent(event_source, event_type, content, is_public);
        };

        Because of = () => result = event_store.Commit(uncommitted_event).Result;
        It should_not_call_the_event_types_with_the_content = () => event_types.Verify(_ => _.GetFor(content.GetType()), Times.Never());
        It should_call_the_converter_with_uncommitted_events = () => converter.Verify(_ => _.ToProtobuf(Moq.It.Is<UncommittedEvents>(_ => _[0] == uncommitted_event)));
        It should_call_the_caller_with_the_correct_request = () => caller.Verify(_ => _.Call(Moq.It.IsAny<EventStoreCommitMethod>(), commit_events_request, Moq.It.IsAny<CancellationToken>()), Times.Once());
        It should_set_the_events_in_the_request = () => commit_events_request.Events.ShouldEqual(pb_uncommitted_events);
        It should_set_the_execution_context_to_the_call_context = () => commit_events_request.CallContext.ExecutionContext.ShouldEqual(execution_context.ToProtobuf());
        It should_call_the_converter_with_results_from_the_caller = () => converter.Verify(_ => _.ToSDK(commit_events_response));
        It should_get_commit_events_result_from_the_converter = () => result.ShouldEqual(commit_events_result);
    }
}

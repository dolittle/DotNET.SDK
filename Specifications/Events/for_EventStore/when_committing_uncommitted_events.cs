// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.SDK.Events.for_EventConverter.given;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Protobuf;
using Machine.Specifications;
using Moq;
using Contracts = Dolittle.Runtime.Events.Contracts;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.Events.for_EventStore
{
    public class when_committing_uncommitted_events : given.an_event_store_and_an_execution_context
    {
        static UncommittedEvents uncommitted_events;
        static CommitEventsResult result;

        Establish context = () =>
        {
            var event_source_one = "e7fe623b-5fb7-4699-9b08-7c14d7556e84";
            var event_type_one = new EventType("4134d0b4-a13f-4c5d-ae98-8e44903ab147", 2);
            var content_one = new an_event("hello wörld", 42, true);
            var is_public_one = true;

            var event_source_two = "d3bc1b39-960b-44b4-a5f2-fa3d8c6c8056";
            var event_type_two = new EventType("da6b65d6-1a8e-4c93-a778-5200a0b7fbbf", 1337);
            var content_two = new an_event("bye wørld", -42, false);
            var is_public_two = false;

            var event_one = new UncommittedEvent(event_source_one, event_type_one, content_one, is_public_one);
            var event_two = new UncommittedEvent(event_source_two, event_type_two, content_two, is_public_two);

            uncommitted_events = new UncommittedEvents
            {
                event_one,
                event_two,
                event_two,
            };
        };

        Because of = () => result = event_store.Commit(uncommitted_events).Result;
        It should_call_the_converter_with_uncommitted_events = () => converter.Verify(_ => _.ToProtobuf(Moq.It.IsAny<UncommittedEvents>()));
        It should_call_the_caller_with_the_correct_request = () => caller.Verify(_ => _.Call(Moq.It.IsAny<EventStoreCommitMethod>(), commit_events_request, Moq.It.IsAny<CancellationToken>()), Times.Once());
        It should_set_the_events_in_the_request = () => commit_events_request.Events.ShouldEqual(pb_uncommitted_events);
        It should_call_the_converter_with_results_from_the_caller = () => converter.Verify(_ => _.ToSDK(commit_events_response));
        It should_set_the_execution_context_to_the_call_context = () => commit_events_request.CallContext.ExecutionContext.ShouldEqual(execution_context.ToProtobuf());
        It should_get_commit_events_result_from_the_converter = () => result.ShouldEqual(commit_events_result);
    }
}
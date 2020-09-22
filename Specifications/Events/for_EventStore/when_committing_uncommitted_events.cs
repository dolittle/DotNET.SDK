// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.SDK.Events.for_EventConverter.given;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Protobuf;
using Machine.Specifications;
using Contracts = Dolittle.Runtime.Events.Contracts;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.Events.for_EventStore
{
    public class when_committing_uncommitted_events : given.an_event_store_and_an_execution_context
    {
        static UncommittedEvents uncommitted_events;

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

        Because of = async () => await event_store.Commit(uncommitted_events);
        It should_have_called_the_caller = () => caller.Verify(_ => _.Call(Moq.It.IsAny<EventStoreCommitMethod>(), Moq.It.IsAny<Contracts.CommitEventsRequest>(), Moq.It.IsAny<CancellationToken>()));
        It should_have_added_the_execution_context_to_the_call_context = () => commit_events_request.CallContext.ExecutionContext.ShouldEqual(execution_context.ToProtobuf());
        It should_have_added_a_not_set_head_id_to_the_call_context = () => commit_events_request.CallContext.HeadId.ShouldEqual(HeadId.NotSet.Value.ToProtobuf());
        It should_have_added_events_to_the_request = () => commit_events_request.Events.Count.ShouldEqual(uncommitted_events.Count);
        It should_have_the_same_event_source_for_the_first_event = () => events_to_convert[0].EventSource.ShouldEqual(uncommitted_events[0].EventSource);
        It should_have_the_same_event_type_for_the_first_event = () => events_to_convert[0].EventType.ShouldEqual(uncommitted_events[0].EventType);
        It should_have_the_same_event_type_generation_for_the_first_event = () => events_to_convert[0].EventType.Generation.ShouldEqual(uncommitted_events[0].EventType.Generation);
        It should_have_the_same_content_for_the_first_event = () => events_to_convert[0].Content.ShouldEqual(uncommitted_events[0].Content);
        It should_have_the_same_is_public_for_the_first_event = () => events_to_convert[0].IsPublic.ShouldEqual(uncommitted_events[0].IsPublic);
        It should_have_the_same_event_source_for_the_second_event = () => events_to_convert[1].EventSource.ShouldEqual(uncommitted_events[1].EventSource);
        It should_have_the_same_event_type_for_the_second_event = () => events_to_convert[1].EventType.ShouldEqual(uncommitted_events[1].EventType);
        It should_have_the_same_event_type_generation_for_the_second_event = () => events_to_convert[1].EventType.Generation.ShouldEqual(uncommitted_events[1].EventType.Generation);
        It should_have_the_same_content_for_the_second_event = () => events_to_convert[1].Content.ShouldEqual(uncommitted_events[1].Content);
        It should_have_the_same_is_public_for_the_second_event = () => events_to_convert[1].IsPublic.ShouldEqual(uncommitted_events[1].IsPublic);
        It should_have_the_same_event_source_for_the_third_event = () => events_to_convert[2].EventSource.ShouldEqual(uncommitted_events[2].EventSource);
        It should_have_the_same_event_type_for_the_third_event = () => events_to_convert[2].EventType.ShouldEqual(uncommitted_events[2].EventType);
        It should_have_the_same_event_type_generation_for_the_third_event = () => events_to_convert[2].EventType.Generation.ShouldEqual(uncommitted_events[2].EventType.Generation);
        It should_have_the_same_content_for_the_third_event = () => events_to_convert[2].Content.ShouldEqual(uncommitted_events[2].Content);
        It should_have_the_same_is_public_for_the_third_event = () => events_to_convert[2].IsPublic.ShouldEqual(uncommitted_events[2].IsPublic);
    }
}

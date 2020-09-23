// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events.for_EventConverter.given;
using Machine.Specifications;
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
        It should_convert_the_object_to_uncommitted_events = () => events_to_convert.ShouldBeOfExactType<UncommittedEvents>();
        It should_have_the_correct_event_source_id = () => events_to_convert[0].EventSource.ShouldEqual(event_source);
        It should_have_the_correct_event_type = () => events_to_convert[0].EventType.ShouldEqual(event_type);
        It should_have_the_correct_content = () => events_to_convert[0].Content.ShouldEqual(content);
        It shouldnt_be_public = () => events_to_convert[0].IsPublic.ShouldBeFalse();
    }
}

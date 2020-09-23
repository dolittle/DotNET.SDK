// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events.for_EventConverter.given;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.Events.for_EventStore
{
    public class when_committing_a_public_event_with_an_event_type : given.an_event_store_and_an_execution_context
    {
        static an_event content;
        static EventSourceId event_source;
        static EventType event_type;

        Establish context = () =>
        {
            content = new an_event("goodbye public world", 12345, true);
            event_source = new EventSourceId("be274a66-e442-455a-b5d9-81d058fc179e");
            event_type = new EventType("31819551-844f-4a6f-935c-917a126531f6");
        };

        Because of = async () => await event_store.CommitPublic(content, event_source, event_type);
        It should_convert_the_object_to_uncommitted_events = () => events_to_convert.ShouldBeOfExactType<UncommittedEvents>();
        It should_have_the_correct_event_source_id = () => events_to_convert[0].EventSource.ShouldEqual(event_source);
        It should_have_the_correct_event_type = () => events_to_convert[0].EventType.ShouldEqual(event_type);
        It should_have_the_correct_content = () => events_to_convert[0].Content.ShouldEqual(content);
        It should_be_public = () => events_to_convert[0].IsPublic.ShouldBeTrue();
    }
}

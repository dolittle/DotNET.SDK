// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Events.for_EventConverter.given;
using Machine.Specifications;
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

        Establish context = () =>
        {
            content = new an_event("goodbye world", 12345, false);
            event_source = new EventSourceId(Guid.NewGuid());
            event_type = new EventType(Guid.NewGuid());
            is_public = false;
            uncommitted_event = new UncommittedEvent(event_source, event_type, content, is_public);
        };

        Because of = async () => await event_store.Commit(uncommitted_event);
        It should_convert_the_object_to_uncommitted_events = () => events_to_convert.ShouldBeOfExactType<UncommittedEvents>();
        It should_have_the_correct_event_source_id = () => events_to_convert[0].EventSource.ShouldEqual(uncommitted_event.EventSource);
        It should_have_the_correct_event_type = () => events_to_convert[0].EventType.ShouldEqual(uncommitted_event.EventType);
        It should_have_the_correct_content = () => events_to_convert[0].Content.ShouldEqual(uncommitted_event.Content);
        It shouldnt_be_public = () => events_to_convert[0].IsPublic.ShouldEqual(uncommitted_event.IsPublic);
    }
}

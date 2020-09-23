// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Events.for_EventConverter.given;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.Events.for_EventStore
{
    public class when_committing_an_event : given.an_event_store_and_an_execution_context
    {
        static an_event content;
        static EventSourceId event_source;
        static EventType event_type;

        Establish context = () =>
        {
            content = new an_event("goodbye world", 12345, true);
            event_source = new EventSourceId(Guid.NewGuid());
            event_type = new EventType(Guid.NewGuid());

            event_types.Setup(_ => _.GetFor(content.GetType())).Returns(event_type);
        };

        Because of = async () => await event_store.Commit(content, event_source);
        It should_convert_the_object_to_uncommitted_events = () => events_to_convert.ShouldBeOfExactType<UncommittedEvents>();
        It should_have_the_correct_event_source_id = () => events_to_convert[0].EventSource.ShouldEqual(event_source);
        It should_have_the_correct_event_type = () => events_to_convert[0].EventType.ShouldEqual(event_type);
        It should_have_the_correct_content = () => events_to_convert[0].Content.ShouldEqual(content);
        It shouldnt_be_public = () => events_to_convert[0].IsPublic.ShouldBeFalse();
    }
}

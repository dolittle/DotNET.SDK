// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.SDK.Events.for_EventConverter.when_converting_to_protobuf.given
{
    public class an_uncomitted_event
    {
        protected static UncommittedEvent uncomitted_event;
        protected static EventSourceId event_source_id;
        protected static EventType event_type;
        protected static an_event @event;

        Establish context = () =>
        {
            event_source_id = EventSourceId.New();
            event_type = new EventType(Guid.NewGuid());
            @event = new an_event { a_string = "Hello World", a_bool = true, an_integer = 42 };
            uncomitted_event = new UncommittedEvent(event_source_id, event_type, @event, false);
        }
    }
}

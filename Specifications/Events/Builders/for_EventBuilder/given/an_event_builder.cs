// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.SDK.Events.Builders.for_EventBuilder.given;

public class an_event_builder
{
    protected static EventBuilder event_builder;
    protected static some_event_type @event;
    protected static EventSourceId event_source_id;
    protected static bool is_public;

    Establish context = () =>
    {
        @event = new some_event_type();
        is_public = false;
        event_source_id = EventSourceId.New();
        event_builder = new EventBuilder(@event, event_source_id, is_public);
    };
}
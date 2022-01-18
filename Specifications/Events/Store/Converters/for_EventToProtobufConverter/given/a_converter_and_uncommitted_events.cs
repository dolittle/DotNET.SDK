// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events.given;
using Dolittle.SDK.Events.Store.Converters.given;
using Machine.Specifications;

namespace Dolittle.SDK.Events.Store.Converters.for_EventToProtobufConverter.given;

public class a_converter_and_uncommitted_events : a_content_serializer_and_an_execution_context
{
    protected static IConvertEventsToProtobuf converter;

    protected static EventSourceId event_source_one;
    protected static EventType event_type_one;
    protected static an_event content_one;
    protected static bool is_public_one;

    protected static EventSourceId event_source_two;
    protected static EventType event_type_two;
    protected static an_event content_two;
    protected static bool is_public_two;

    protected static string content_as_string_one;
    protected static string content_as_string_two;

    protected static UncommittedEvents uncommitted_events;

    Establish context = () =>
    {
        converter = new EventToProtobufConverter(serializer.Object);

        event_source_one = "Mamie Burke";
        event_type_one = new EventType("4134d0b4-a13f-4c5d-ae98-8e44903ab147", 2);
        content_one = new an_event("hello world", 42, true);
        is_public_one = true;
        content_as_string_one = "first event test content string";

        event_source_two = "d3bc1b39-960b-44b4-a5f2-fa3d8c6c8056";
        event_type_two = new EventType("da6b65d6-1a8e-4c93-a778-5200a0b7fbbf", 1337);
        content_two = new an_event("bye wørld", -42, false);
        is_public_two = false;
        content_as_string_two = "second event test content string";

        var event_one = new UncommittedEvent(event_source_one, event_type_one, content_one, is_public_one);
        var event_two = new UncommittedEvent(event_source_two, event_type_two, content_two, is_public_two);

        uncommitted_events = new UncommittedEvents
        {
            event_one,
            event_two,
            event_two
        };
    };
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.SDK.Events.for_EventConverter.given;
using Dolittle.SDK.Protobuf;
using Machine.Specifications;
using Newtonsoft.Json;
using PbUncommittedEvent = Dolittle.Runtime.Events.Contracts.UncommittedEvent;

namespace Dolittle.SDK.Events.for_EventConverter
{
    public class when_converting_uncommitted_events_to_protobuf : given.a_converter
    {
        static EventSourceId event_source_one;
        static EventType event_type_one;
        static an_event content_one;
        static bool is_public_one;

        static EventSourceId event_source_two;
        static EventType event_type_two;
        static an_event content_two;
        static bool is_public_two;

        static string content_as_json_one;
        static string content_as_json_two;

        static UncommittedEvents uncommitted_events;

        static PbUncommittedEvent[] converted_uncommitted_events;

        Establish context = () =>
        {
            event_source_one = "e7fe623b-5fb7-4699-9b08-7c14d7556e84";
            event_type_one = new EventType("4134d0b4-a13f-4c5d-ae98-8e44903ab147", 2);
            content_one = new an_event("hello world", 42, true);
            is_public_one = true;

            content_as_json_one = JsonConvert.SerializeObject(content_one);

            event_source_two = "d3bc1b39-960b-44b4-a5f2-fa3d8c6c8056";
            event_type_two = new EventType("da6b65d6-1a8e-4c93-a778-5200a0b7fbbf", 1337);
            content_two = new an_event("bye wørld", -42, false);
            is_public_two = false;

            content_as_json_two = JsonConvert.SerializeObject(content_two);

            var event_one = new UncommittedEvent(event_source_one, event_type_one, content_one, is_public_one);
            var event_two = new UncommittedEvent(event_source_two, event_type_two, content_two, is_public_two);

            uncommitted_events = new UncommittedEvents
            {
                event_one,
                event_two,
                event_two,
            };
        };

        Because of = () => converted_uncommitted_events = converter.ToProtobuf(uncommitted_events).ToArray();

        It should_return_three_events = () => converted_uncommitted_events.Length.ShouldEqual(3);
        It should_have_the_same_event_source_for_the_first_event = () => converted_uncommitted_events[0].EventSourceId.ShouldEqual(event_source_one.ToProtobuf());
        It should_have_the_same_event_type_id_for_the_first_event = () => converted_uncommitted_events[0].Artifact.Id.ShouldEqual(event_type_one.Id.ToProtobuf());
        It should_have_the_same_event_type_generation_for_the_first_event = () => converted_uncommitted_events[0].Artifact.Generation.ShouldEqual(event_type_one.Generation.Value);
        It should_serialize_the_content_as_json_for_the_first_event = () => converted_uncommitted_events[0].Content.ShouldEqual(content_as_json_one);
        It should_have_the_same_is_public_for_the_first_event = () => converted_uncommitted_events[0].Public.ShouldEqual(is_public_one);
        It should_have_the_same_event_source_for_the_second_event = () => converted_uncommitted_events[1].EventSourceId.ShouldEqual(event_source_two.ToProtobuf());
        It should_have_the_same_event_type_id_for_the_second_event = () => converted_uncommitted_events[1].Artifact.Id.ShouldEqual(event_type_two.Id.ToProtobuf());
        It should_have_the_same_event_type_generation_for_the_second_event = () => converted_uncommitted_events[1].Artifact.Generation.ShouldEqual(event_type_two.Generation.Value);
        It should_serialize_the_content_as_json_for_the_second_event = () => converted_uncommitted_events[1].Content.ShouldEqual(content_as_json_two);
        It should_have_the_same_is_public_for_the_second_event = () => converted_uncommitted_events[1].Public.ShouldEqual(is_public_two);
        It should_have_the_same_event_source_for_the_third_event = () => converted_uncommitted_events[2].EventSourceId.ShouldEqual(event_source_two.ToProtobuf());
        It should_have_the_same_event_type_id_for_the_third_event = () => converted_uncommitted_events[2].Artifact.Id.ShouldEqual(event_type_two.Id.ToProtobuf());
        It should_have_the_same_event_type_generation_for_the_third_event = () => converted_uncommitted_events[2].Artifact.Generation.ShouldEqual(event_type_two.Generation.Value);
        It should_serialize_the_content_as_json_for_the_third_event = () => converted_uncommitted_events[2].Content.ShouldEqual(content_as_json_two);
        It should_have_the_same_is_public_for_the_third_event = () => converted_uncommitted_events[2].Public.ShouldEqual(is_public_two);
    }
}
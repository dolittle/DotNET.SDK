// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events.for_EventConverter.given;
using Dolittle.SDK.Protobuf;
using Machine.Specifications;
using Newtonsoft.Json;
using PbUncommittedEvent = Dolittle.Runtime.Events.Contracts.UncommittedEvent;

namespace Dolittle.SDK.Events.for_EventConverter
{
    public class when_converting_an_uncommitted_event_to_protobuf : given.a_converter
    {
        static EventSourceId event_source;
        static EventType event_type;
        static an_event content;
        static bool is_public;

        static UncommittedEvent uncommitted_event;
        static string content_as_json;

        static PbUncommittedEvent converted_uncommitted_event;

        Establish context = () =>
        {
            event_source = "e7fe623b-5fb7-4699-9b08-7c14d7556e84";
            event_type = new EventType("4134d0b4-a13f-4c5d-ae98-8e44903ab147", 2);
            content = new an_event("hello world", 42, true);
            is_public = true;

            uncommitted_event = new UncommittedEvent(event_source, event_type, content, is_public);

            content_as_json = JsonConvert.SerializeObject(content);
        };

        Because of = () => converted_uncommitted_event = converter.ToProtobuf(uncommitted_event);

        It should_have_the_same_event_source = () => converted_uncommitted_event.EventSourceId.ShouldEqual(event_source.ToProtobuf());
        It should_have_the_same_event_type_id = () => converted_uncommitted_event.Artifact.Id.ShouldEqual(event_type.Id.ToProtobuf());
        It should_have_the_same_event_type_generation = () => converted_uncommitted_event.Artifact.Generation.ShouldEqual(event_type.Generation.Value);
        It should_serialize_the_content_as_json = () => converted_uncommitted_event.Content.ShouldEqual(content_as_json);
        It should_have_the_same_is_public = () => converted_uncommitted_event.Public.ShouldEqual(is_public);
    }
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.SDK.Events.given;
using Dolittle.SDK.Protobuf;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;
using PbUncommittedEvent = Dolittle.Runtime.Events.Contracts.UncommittedEvent;

namespace Dolittle.SDK.Events.Store.Converters.for_EventToProtobufConverter
{
    public class when_converting_uncommitted_events : given.a_converter
    {
        static EventSourceId event_source_one;
        static EventType event_type_one;
        static an_event content_one;
        static bool is_public_one;

        static EventSourceId event_source_two;
        static EventType event_type_two;
        static an_event content_two;
        static bool is_public_two;

        static string content_as_string_one;
        static string content_as_string_two;

        static UncommittedEvents uncommitted_events;

        static IEnumerable<PbUncommittedEvent> converted_uncommitted_events;
        static Exception exception;
        static bool try_result;

        delegate void TryToSerializeCallback(object source, out string jsonString, out Exception error);

        Establish context = () =>
        {
            event_source_one = "e7fe623b-5fb7-4699-9b08-7c14d7556e84";
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
                event_two
            };

            serializer
                .Setup(_ => _.TryToSerialize(event_one.Content, out Moq.It.Ref<string>.IsAny, out Moq.It.Ref<Exception>.IsAny))
                .Callback(new TryToSerializeCallback((object _, out string jsonString, out Exception error) =>
                    {
                        jsonString = content_as_string_one;
                        error = null;
                    }))
                .Returns(true);
            serializer
                .Setup(_ => _.TryToSerialize(event_two.Content, out Moq.It.Ref<string>.IsAny, out Moq.It.Ref<Exception>.IsAny))
                .Callback(new TryToSerializeCallback((object _, out string jsonString, out Exception error) =>
                    {
                        jsonString = content_as_string_two;
                        error = null;
                    }))
                .Returns(true);
        };

        Because of = () => try_result = converter.TryToProtobuf(uncommitted_events, out converted_uncommitted_events, out exception);

        It should_return_true = () => try_result.ShouldBeTrue();
        It should_have_no_exception = () => exception.ShouldBeNull();
        It should_return_three_events = () => converted_uncommitted_events.Count().ShouldEqual(uncommitted_events.Count);
        It should_have_the_same_event_source_for_the_first_event = () => converted_uncommitted_events.ElementAt(0).EventSourceId.ShouldEqual(event_source_one.ToProtobuf());
        It should_have_the_same_event_type_id_for_the_first_event = () => converted_uncommitted_events.ElementAt(0).Artifact.Id.ShouldEqual(event_type_one.Id.ToProtobuf());
        It should_have_the_same_event_type_generation_for_the_first_event = () => converted_uncommitted_events.ElementAt(0).Artifact.Generation.ShouldEqual(event_type_one.Generation.Value);
        It should_have_called_the_serializer_on_the_first_events_content = () => serializer.Verify(_ => _.TryToSerialize(uncommitted_events.ElementAt(0).Content, out Moq.It.Ref<string>.IsAny, out Moq.It.Ref<Exception>.IsAny), Times.Once());
        It should_have_content_set_by_the_serializer_on_the_first_event = () => converted_uncommitted_events.ElementAt(0).Content.ShouldEqual(content_as_string_one);
        It should_have_the_same_is_public_for_the_first_event = () => converted_uncommitted_events.ElementAt(0).Public.ShouldEqual(is_public_one);
        It should_have_the_same_event_source_for_the_second_event = () => converted_uncommitted_events.ElementAt(1).EventSourceId.ShouldEqual(event_source_two.ToProtobuf());
        It should_have_the_same_event_type_id_for_the_second_event = () => converted_uncommitted_events.ElementAt(1).Artifact.Id.ShouldEqual(event_type_two.Id.ToProtobuf());
        It should_have_the_same_event_type_generation_for_the_second_event = () => converted_uncommitted_events.ElementAt(1).Artifact.Generation.ShouldEqual(event_type_two.Generation.Value);
        It should_have_called_the_serializer_on_the_second_events_content = () => serializer.Verify(_ => _.TryToSerialize(uncommitted_events.ElementAt(1).Content, out Moq.It.Ref<string>.IsAny, out Moq.It.Ref<Exception>.IsAny), Times.Once());
        It should_have_content_set_by_the_serializer_on_the_second_event = () => converted_uncommitted_events.ElementAt(1).Content.ShouldEqual(content_as_string_two);
        It should_have_the_same_is_public_for_the_second_event = () => converted_uncommitted_events.ElementAt(1).Public.ShouldEqual(is_public_two);
    }
}

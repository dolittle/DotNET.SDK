// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;
using PbCommittedEvent = Dolittle.Runtime.Events.Contracts.CommittedEvent;

namespace Dolittle.SDK.Events.Store.Converters.for_EventToSDKConverter.when_converting_multiple_committed_events
{
    public class and_second_event_is_invalid : given.a_committed_event_and_a_converter
    {
        static PbCommittedEvent second_committed_event;

        static object object_from_serializer;

        static bool try_result;
        static CommittedEvents converted_events;
        static Exception exception;

        Establish context = () =>
        {
            second_committed_event = new PbCommittedEvent();

            object_from_serializer = new object();
            SetupDeserializeToReturnObject(content_string, object_from_serializer);
        };

        Because of = () => try_result = converter.TryConvert(new[] { committed_event, second_committed_event }, out converted_events, out exception);

        It should_return_false = () => try_result.ShouldBeFalse();
        It should_out_default_event = () => converted_events.ShouldEqual(default);
        It should_out_an_exception = () => exception.ShouldNotBeNull();
    }
}

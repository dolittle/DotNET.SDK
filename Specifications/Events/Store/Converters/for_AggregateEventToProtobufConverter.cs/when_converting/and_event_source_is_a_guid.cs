// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Protobuf;
using Machine.Specifications;
using PbUncommittedAggregateEvents = Dolittle.Runtime.Events.Contracts.UncommittedAggregateEvents;

namespace Dolittle.SDK.Events.Store.Converters.for_AggregateEventToProtobufConverter.when_converting
{
    public class and_event_source_is_a_guid : given.a_converter_and_uncommitted_events
    {
        static Guid event_source_guid;

        static PbUncommittedAggregateEvents converted_uncommitted_events;
        static Exception exception;
        static bool try_result;

        Establish context = () =>
        {
            event_source_guid = Guid.Parse("0b956ed9-0126-4447-8647-e41348dafdb8");
            event_source_id = event_source_guid;

            uncommitted_aggregate_events = new UncommittedAggregateEvents(event_source_id, aggregate_root_id, aggregate_root_version)
            {
                event_one,
                event_two,
                event_two
            };

            SetupSerializeToReturnJSON(content_one, content_as_string_one);
            SetupSerializeToReturnJSON(content_two, content_as_string_two);
        };

        Because of = () => try_result = converter.TryConvert(uncommitted_aggregate_events, out converted_uncommitted_events, out exception);

        It should_return_true = () => try_result.ShouldBeTrue();
        It should_have_no_exception = () => exception.ShouldBeNull();
        It shouldshould_have_the_correct_event_source = () => converted_uncommitted_events.EventSourceId.ShouldEqual(event_source_guid.ToString());
    }
}

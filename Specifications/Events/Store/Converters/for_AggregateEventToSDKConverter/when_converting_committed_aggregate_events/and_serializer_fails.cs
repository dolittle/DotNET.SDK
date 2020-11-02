// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.SDK.Events.Store.Converters.for_AggregateEventToSDKConverter.when_converting_committed_aggregate_events
{
    public class and_serializer_fails : given.committed_aggregate_events_and_a_converter
    {
        static Exception serializer_exception;

        static bool try_result;
        static CommittedAggregateEvents converted_events;
        static Exception exception;

        Establish context = () =>
        {
            serializer_exception = new Exception();
            SetupDeserializeToFail(content_string, serializer_exception);
        };

        Because of = () => try_result = converter.TryConvert(committed_aggregate_events, out converted_events, out exception);

        It should_return_false = () => try_result.ShouldBeFalse();
        It should_out_default_events = () => converted_events.ShouldEqual(default);
        It should_out_invalid_committed_event_information = () => exception.ShouldBeOfExactType<InvalidCommittedEventInformation>();
        It should_out_invalid_committed_event_information_with_serializer_inner_exception = () => exception.InnerException.ShouldBeTheSameAs(serializer_exception);
    }
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;
using It = Machine.Specifications.It;
using PbUncommittedAggregateEvents = Dolittle.Runtime.Events.Contracts.UncommittedAggregateEvents;

namespace Dolittle.SDK.Events.Store.Converters.for_AggregateEventToProtobufConverter.when_converting
{
    public class and_serializer_fails : given.a_converter_and_uncommitted_events
    {
        static Exception serializer_exception;

        static PbUncommittedAggregateEvents converted_uncommitted_aggregate_events;
        static Exception exception;
        static bool try_result;

        Establish context = () =>
        {
            serializer_exception = new Exception();
            SetupSerializeToReturnJSON(content_one, "");
            SetupSerializeToFail(content_two, serializer_exception);
        };

        Because of = () => try_result = converter.TryConvert(uncommitted_aggregate_events, out converted_uncommitted_aggregate_events, out exception);

        It should_return_false = () => try_result.ShouldBeFalse();
        It should_out_default_events = () => converted_uncommitted_aggregate_events.ShouldEqual(default);
        It should_out_serializer_exception = () => exception.ShouldBeTheSameAs(serializer_exception);
    }
}

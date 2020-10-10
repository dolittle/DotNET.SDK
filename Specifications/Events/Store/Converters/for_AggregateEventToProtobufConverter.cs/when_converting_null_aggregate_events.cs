// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;
using It = Machine.Specifications.It;
using PbUncommittedAggregateEvents = Dolittle.Runtime.Events.Contracts.UncommittedAggregateEvents;

namespace Dolittle.SDK.Events.Store.Converters.for_AggregateEventToProtobufConverter
{
    public class when_converting_null_aggregate_events : given.a_converter
    {
        static PbUncommittedAggregateEvents converted_uncommitted_aggregate_events;
        static Exception exception;
        static bool try_result;

        Because of = () => try_result = converter.TryToProtobuf(null, out converted_uncommitted_aggregate_events, out exception);

        It should_return_false = () => try_result.ShouldBeFalse();
        It should_out_default_events = () => converted_uncommitted_aggregate_events.ShouldEqual(default);
        It should_out_null_exception = () => exception.ShouldBeOfExactType<ArgumentNullException>();
    }
}

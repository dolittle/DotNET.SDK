// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.SDK.Events.Store.Converters.for_AggregateEventToSDKConverter.when_converting_committed_aggregate_events;

public class and_events_is_null : given.a_converter_and_a_protobuf_execution_context
{
    static bool try_result;
    static CommittedAggregateEvents converted_events;
    static Exception exception;

    Because of = () => try_result = converter.TryConvert(null, out converted_events, out exception);

    It should_return_false = () => try_result.ShouldBeFalse();
    It should_out_default_events = () => converted_events.ShouldEqual(default);
    It should_out_null_exception = () => exception.ShouldBeOfExactType<ArgumentNullException>();
}
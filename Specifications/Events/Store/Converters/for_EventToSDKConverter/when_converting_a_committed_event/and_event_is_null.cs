// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.SDK.Events.Store.Converters.for_EventToSDKConverter.when_converting_a_committed_event;

public class and_event_is_null : given.a_committed_event_and_a_converter
{
    static bool try_result;
    static CommittedEvent converted_event;
    static Exception exception;

    Because of = () => try_result = converter.TryConvert(null, out converted_event, out exception);

    It should_return_false = () => try_result.ShouldBeFalse();
    It should_out_default_event = () => converted_event.ShouldEqual(default);
    It should_out_null_exception = () => exception.ShouldBeOfExactType<ArgumentNullException>();
}
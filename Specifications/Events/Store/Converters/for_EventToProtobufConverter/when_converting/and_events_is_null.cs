// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Machine.Specifications;
using It = Machine.Specifications.It;
using PbUncommittedEvent = Dolittle.Runtime.Events.Contracts.UncommittedEvent;

namespace Dolittle.SDK.Events.Store.Converters.for_EventToProtobufConverter.when_converting;

public class and_events_is_null : given.a_converter_and_uncommitted_events
{
    static IReadOnlyList<PbUncommittedEvent> converted_uncommitted_events;
    static Exception exception;
    static bool try_result;

    Because of = () => try_result = converter.TryConvert(null, out converted_uncommitted_events, out exception);

    It should_return_false = () => try_result.ShouldBeFalse();
    It should_out_default_events = () => converted_uncommitted_events.ShouldEqual(default);
    It should_out_null_exception = () => exception.ShouldBeOfExactType<ArgumentNullException>();
}
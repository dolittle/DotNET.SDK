// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Machine.Specifications;
using It = Machine.Specifications.It;
using PbUncommittedEvent = Dolittle.Runtime.Events.Contracts.UncommittedEvent;

namespace Dolittle.SDK.Events.Store.Converters.for_EventToProtobufConverter.when_converting;

public class and_serializer_fails : given.a_converter_and_uncommitted_events
{
    static Exception serializer_exception;

    static IReadOnlyList<PbUncommittedEvent> converted_uncommitted_events;
    static Exception exception;
    static bool try_result;

    Establish context = () =>
    {
        serializer_exception = new Exception();
        SetupSerializeToReturnJSON(content_one, "");
        SetupSerializeToFail(content_two, serializer_exception);
    };

    Because of = () => try_result = converter.TryConvert(uncommitted_events, out converted_uncommitted_events, out exception);

    It should_return_false = () => try_result.ShouldBeFalse();
    It should_out_default_events = () => converted_uncommitted_events.ShouldEqual(default);
    It should_out_serializer_exception = () => exception.ShouldBeTheSameAs(serializer_exception);
}
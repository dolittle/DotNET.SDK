// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.SDK.Events.Store.Converters.for_EventToSDKConverter.when_converting_a_committed_event;

public class and_serializer_fails : given.a_committed_event_and_a_converter
{
    static Exception exception_from_serializer;

    static bool try_result;
    static CommittedEvent converted_event;
    static Exception exception;

    Establish context = () =>
    {
        exception_from_serializer = new Exception();
        SetupDeserializeToFail(content_string, exception_from_serializer);
    };

    Because of = () => try_result = converter.TryConvert(committed_event, out converted_event, out exception);

    It should_return_false = () => try_result.ShouldBeFalse();
    It should_out_default_event = () => converted_event.ShouldEqual(default);
    It should_out_invalid_committed_event_information = () => exception.ShouldBeOfExactType<InvalidCommittedEventInformation>();
    It should_out_invalid_committed_event_information_with_serializer_inner_exception = () => exception.InnerException.ShouldBeTheSameAs(exception_from_serializer);
}
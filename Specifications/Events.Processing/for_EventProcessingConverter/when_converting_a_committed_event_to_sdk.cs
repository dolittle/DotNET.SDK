// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Security;
using Machine.Specifications;
using PbCommittedEvent = Dolittle.Runtime.Events.Contracts.CommittedEvent;

namespace Dolittle.SDK.Events.Processing.for_EventProcessingConverter
{
    public class when_converting_a_committed_event_to_sdk : given.a_converter
    {
        static PbCommittedEvent committed_event;
        static CommittedEvent committed_event_to_return;

        static CommittedEvent converted_committed_event;

        Establish context = () =>
        {
            committed_event = new PbCommittedEvent();
            committed_event_to_return = new CommittedEvent(
                506549693,
                new DateTimeOffset(2020, 2, 20, 20, 20, 20, TimeSpan.Zero),
                "cc1326fc-ca2f-4ad4-a9fb-53ad9e771ee8",
                new ExecutionContext(
                    "388ed676-2448-45fe-99c7-93094e8241c4",
                    "6714619f-b2dd-4d90-a545-642df1bbb67b",
                    new Microservices.Version(5, 4, 3, 2, "duvorwuicu"),
                    "latmazokcu",
                    "a6d990c8-81cb-4a8b-b7cf-327c83ce7cc4",
                    new Claims(new[]
                    {
                        new Claim("bibetudicu", "efajeakuga", "wovawjohoa"),
                    }),
                    CultureInfo.InvariantCulture),
                new EventType("836faf72-6212-4351-adad-89bc1d7964c6", 3421559541),
                new object(),
                true);

            event_converter.Setup(_ => _.ToSDK(committed_event)).Returns(committed_event_to_return);
        };

        Because of = () => converted_committed_event = event_processing_converter.ToSDK(committed_event);

        It should_call_the_event_converter_with_the_committed_event = () => event_converter.Verify(_ => _.ToSDK(committed_event));
        It should_return_the_result_from_the_event_converter = () => converted_committed_event.ShouldBeTheSameAs(committed_event_to_return);
        It should_not_call_the_event_converter_for_anything_else = () => event_converter.VerifyNoOtherCalls();
    }
}
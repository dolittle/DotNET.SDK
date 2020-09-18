// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Security;
using Machine.Specifications;
using Version = Dolittle.SDK.Microservices.Version;

namespace Dolittle.SDK.Events.for_CommittedEventExtensions
{
    public class when_getting_event_context
    {
        static CommittedEvent committed_event;

        static EventContext event_context;

        Establish context = () =>
        {
            committed_event = new CommittedEvent(
                2432633885,
                new DateTimeOffset(1234, 12, 23, 1, 2, 3, TimeSpan.Zero),
                "0c8dd4a9-3096-4998-8c02-066395f49836",
                new ExecutionContext(
                    "78565a75-4a75-4d47-9375-8f1be76de8c5",
                    "4a22142d-ea4f-4f6e-a6e2-c85254cee8f3",
                    new Version(1, 2, 3, 4, "likpaudvie"),
                    "wasovurozevp",
                    "a40674d1-7aff-4bdc-84c7-e89869ae90de",
                    new Claims(new[]
                    {
                        new Claim("zizgukouwl", "kegefobeci", "kiajrabija")
                    }),
                    CultureInfo.InvariantCulture),
                new EventType("c2bf94fd-491f-494a-bbf6-f3ef397bf5fc", 76),
                new object(),
                false);
        };

        Because of = () => event_context = committed_event.GetEventContext();

        It should_have_the_same_sequence_number = () => event_context.SequenceNumber.ShouldEqual(committed_event.EventLogSequenceNumber);
        It should_have_the_same_event_source = () => event_context.EventSourceId.ShouldEqual(committed_event.EventSource);
        It should_have_the_same_occured = () => event_context.Occurred.ShouldEqual(committed_event.Occurred);
        It should_have_the_same_execution_context = () => event_context.ExecutionContext.ShouldEqual(committed_event.ExecutionContext);
    }
}
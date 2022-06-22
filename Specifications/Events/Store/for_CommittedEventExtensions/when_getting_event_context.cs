// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Globalization;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Security;
using Machine.Specifications;
using Version = Dolittle.SDK.Microservices.Version;

namespace Dolittle.SDK.Events.Store.for_CommittedEventExtensions;

public class when_getting_event_context
{
    static CommittedEvent committed_event;
    static ExecutionContext current_execution_context;

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
                CultureInfo.InvariantCulture,
                ActivitySpanId.CreateFromString("b00db00db00cb00a")),
            new EventType("c2bf94fd-491f-494a-bbf6-f3ef397bf5fc", 76),
            new object(),
            false);

        current_execution_context = new ExecutionContext(
            "6f4a5d2d-ab2b-4f93-a394-af82748f8904",
            "53a6767f-6fe9-4abf-8408-03dd6712b04f",
            new Version(5, 6, 7, 8, "feuhumewogsikeo"),
            "hedcarojizanafs",
            "638c09e1-b127-4c77-88a5-f0a21efacd36",
            new Claims(new[]
            {
                new Claim("janduthiwl", "wivjuwpesm", "vohocecouw")
            }),
            CultureInfo.InvariantCulture,
            ActivitySpanId.CreateFromString("b00cb00cb00cb00c"));
    };

    Because of = () => event_context = committed_event.GetEventContext(current_execution_context);

    It should_have_the_same_sequence_number = () => event_context.SequenceNumber.ShouldEqual(committed_event.EventLogSequenceNumber);
    It should_have_the_same_event_source = () => event_context.EventSourceId.ShouldEqual(committed_event.EventSource);
    It should_have_the_same_occured = () => event_context.Occurred.ShouldEqual(committed_event.Occurred);
    It should_have_the_correct_committed_execution_context = () => event_context.CommittedExecutionContext.ShouldEqual(committed_event.ExecutionContext);
    It should_have_the_correct_current_execution_context = () => event_context.CurrentExecutionContext.ShouldEqual(current_execution_context);
}
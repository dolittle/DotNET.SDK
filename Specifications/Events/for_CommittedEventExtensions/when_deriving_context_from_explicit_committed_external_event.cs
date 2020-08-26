// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;
using given = Dolittle.Events.given;

namespace Dolittle.Events.for_CommittedEventExtensions
{
    public class when_deriving_context_from_explicit_committed_external_event : given::Events
    {
        static CommittedExternalEvent committed_event;
        static EventLogSequenceNumber sequence_number;
        static DateTimeOffset occurred;
        static EventSourceId event_source;
        static EventLogSequenceNumber external_sequence_number;
        static DateTimeOffset received;
        static ExternalEventContext result;

        Establish context = () =>
        {
            sequence_number = 1729;
            occurred = DateTimeOffset.Now.AddMonths(-12);
            event_source = Guid.Parse("e5bd8861-1821-4658-8a72-71bf97469ea9");
            external_sequence_number = 496;
            received = DateTimeOffset.Now.AddDays(-7);
            committed_event = new CommittedExternalEvent(sequence_number, occurred, event_source, execution_context, external_sequence_number, received, event_one);
        };

        Because of = () => result = committed_event.DeriveContext();

        It should_have_the_correct_sequence_number = () => result.SequenceNumber.ShouldEqual(sequence_number);
        It should_have_the_correct_event_source = () => result.EventSourceId.ShouldEqual(event_source);
        It should_have_the_correct_timestamp = () => result.Occurred.ShouldEqual(occurred);
        It should_have_the_correct_execution_context = () => result.ExecutionContext.ShouldEqual(execution_context);
        It should_have_the_correct_unique_identifier = () => result.UniqueIdentifier.Value.ShouldEqual("FvBR8iLWpE2bH34HjMicgGmkF77U7Q1LvCsgbQteafDwAQAAAAAAAA==");
        It should_have_the_correct_external_sequence_number = () => result.ExternalEventLogSequenceNumber.ShouldEqual(external_sequence_number);
        It should_have_the_correct_received_timestamp = () => result.Received.ShouldEqual(received);
    }
}
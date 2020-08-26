// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;
using given = Dolittle.Events.given;

namespace Dolittle.Events.for_CommittedEventExtensions
{
    public class when_deriving_context_from_committed_event : given::Events
    {
        static CommittedEvent committed_event;
        static EventLogSequenceNumber sequence_number;
        static DateTimeOffset occurred;
        static EventSourceId event_source;
        static EventContext result;

        Establish context = () =>
        {
            sequence_number = 1729;
            occurred = DateTimeOffset.Now.AddMonths(-1);
            event_source = Guid.Parse("e5bd8861-1821-4658-8a72-71bf97469ea9");
            committed_event = new CommittedEvent(sequence_number, occurred, event_source, execution_context, event_one);
        };

        Because of = () => result = committed_event.DeriveContext();

        It should_be_an_event_context = () => result.ShouldBeOfExactType<EventContext>();
        It should_have_the_correct_sequence_number = () => result.SequenceNumber.ShouldEqual(sequence_number);
        It should_have_the_correct_event_source = () => result.EventSourceId.ShouldEqual(event_source);
        It should_have_the_correct_timestamp = () => result.Occurred.ShouldEqual(occurred);
        It should_have_the_correct_execution_context = () => result.ExecutionContext.ShouldEqual(execution_context);
        It should_have_the_correct_unique_identifier = () => result.UniqueIdentifier.Value.ShouldEqual("FvBR8iLWpE2bH34HjMicgGmkF77U7Q1LvCsgbQteafDBBgAAAAAAAA==");
    }
}
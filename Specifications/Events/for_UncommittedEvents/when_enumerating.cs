// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Machine.Specifications;

namespace Dolittle.Events.for_UncommittedEvents
{
    public class when_enumerating : given.Events
    {
        static UncommittedEvents events;
        static UncommittedEvent[] enumerated;
        static UncommittedEvent uncomitted_one;
        static UncommittedEvent uncomitted_two;

        Establish context = () =>
        {
            uncomitted_one = new UncommittedEvent(Guid.Parse("ffbaa652-cf7c-49d1-bbce-810ac0e4234b"), event_one);
            uncomitted_two = new UncommittedEvent(Guid.Parse("2a7bffd6-b61a-4c71-8a7d-a5b0c2945469"), event_two);
            events = new UncommittedEvents();
            events.Append(uncomitted_one);
            events.Append(uncomitted_two);
        };

        Because of = () => enumerated = events.ToArray();

        It should_enumerate_two_events = () => enumerated.Length.ShouldEqual(2);
        It should_enumerate_the_first_event_first = () => enumerated[0].ShouldEqual(uncomitted_one);
        It should_enumerate_the_second_event_second = () => enumerated[1].ShouldEqual(uncomitted_two);
    }
}

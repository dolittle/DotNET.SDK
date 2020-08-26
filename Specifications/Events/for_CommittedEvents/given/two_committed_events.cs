// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;
using given = Dolittle.Events.given;

namespace Dolittle.Events.for_CommittedEvents.given
{
    public abstract class two_committed_events : given::Events
    {
        public static CommittedEvent first_event;
        public static CommittedEvent second_event;

        Establish context = () =>
        {
            first_event = new CommittedEvent(0, DateTimeOffset.Now, Guid.Parse("ce68632b-c5bb-4cf1-8faf-649269b75e9d"), execution_context, event_one);

            second_event = new CommittedEvent(1, DateTimeOffset.Now, Guid.Parse("b688d79e-d487-486c-8348-f42a792579eb"), execution_context, event_two);
        };
    }
}
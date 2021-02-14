// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Aggregates.given;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Builders;
using Machine.Specifications;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Aggregates.for_AggregateRoot.given
{
    public abstract class events_and_event_types
    {
        protected static IEventTypes event_types;
        protected static EventType event_type_for_an_event;
        protected static EventType event_type_for_another_event;
        protected static AnEvent first_event;
        protected static AnEvent second_event;
        protected static AnotherEvent third_event;

        Establish context = () =>
        {
            event_types = new EventTypes(Moq.Mock.Of<ILogger>());
            event_type_for_an_event = new EventType("bdddf647-5043-4ac5-a89e-7fb39bdbc8f1");
            event_type_for_another_event = new EventType("ce0f2d26-903d-43bf-872c-492da728e793");
            var event_types_builder = new EventTypesBuilder();
            event_types_builder.Associate<AnEvent>(event_type_for_an_event);
            event_types_builder.Associate<AnotherEvent>(event_type_for_another_event);
            event_types_builder.AddAssociationsInto(event_types);

            first_event = new AnEvent { SomeString = "some string" };
            second_event = new AnEvent { SomeString = "some other string" };
            third_event = new AnotherEvent { SomeOtherString = "some string" };
        };
    }
}

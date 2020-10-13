// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events.Builders.for_EventBuilder.given;
using Machine.Specifications;
using Moq;

namespace Dolittle.SDK.Events.Builders.for_EventBuilder.when_building_event.that_is_not_associated_with_an_event_type.given
{
    public class all_dependencies : an_event_builder
    {
        protected static Mock<IEventTypes> event_types;
        Establish context = () =>
        {
            event_types = new Mock<IEventTypes>();
            event_types.Setup(_ => _.HasFor(typeof(some_event_type))).Returns(false);
        };
    }
}

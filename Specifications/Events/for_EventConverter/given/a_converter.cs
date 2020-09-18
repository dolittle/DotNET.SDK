// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using Moq;

namespace Dolittle.SDK.Events.for_EventConverter.given
{
    public class a_converter
    {
        protected static Mock<IEventTypes> event_types;
        protected static IEventConverter converter;

        Establish context = () =>
        {
            event_types = new Mock<IEventTypes>();
            converter = new EventConverter(event_types.Object);
        };
    }
}

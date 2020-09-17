// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using Moq;

namespace Dolittle.SDK.Events.Processing.for_EventProcessingRequestConverter.given
{
    public class a_converter
    {
        protected static Mock<IEventTypes> event_types;
        protected static IEventProcessingRequestConverter converter;

        Establish context = () =>
        {
            event_types = new Mock<IEventTypes>();
            converter = new EventProcessingRequestConverter(event_types.Object);
        };
    }
}
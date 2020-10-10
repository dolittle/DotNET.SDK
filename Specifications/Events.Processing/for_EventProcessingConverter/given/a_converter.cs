// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events.Store.Converters;
using Machine.Specifications;
using Moq;

namespace Dolittle.SDK.Events.Processing.for_EventProcessingConverter.given
{
    public class a_converter
    {
        protected static Mock<IConvertEventResponsesToSDK> event_converter;
        protected static EventProcessingConverter event_processing_converter;

        Establish context = () =>
        {
            event_converter = new Mock<IConvertEventResponsesToSDK>();
            event_processing_converter = new EventProcessingConverter(event_converter.Object);
        };
    }
}

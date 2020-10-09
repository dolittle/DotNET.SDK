// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using Moq;

namespace Dolittle.SDK.Events.for_AggregateEventToProtobufConverter.given
{
    public class a_converter
    {
        protected static Mock<ISerializeEventContent> serializer;
        protected static IConvertAggregateEventsToProtobuf converter;

        Establish context = () =>
        {
            serializer = new Mock<ISerializeEventContent>();
            converter = new AggregateEventToProtobufConverter(serializer.Object);
        };
    }
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using Moq;

namespace Dolittle.SDK.Events.Store.Converters.for_EventResponseToSDKConverter.given
{
    public class a_converter
    {
        protected static Mock<ISerializeEventContent> serializer;
        protected static IConvertEventResponsesToSDK converter;

        Establish context = () =>
        {
            serializer = new Mock<ISerializeEventContent>();
            converter = new EventResponseToSDKConverter(serializer.Object);
        };
    }
}

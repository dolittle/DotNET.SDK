// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events.Store.Converters.given;
using Machine.Specifications;

namespace Dolittle.SDK.Events.Store.Converters.for_EventToProtobufConverter.given
{
    public class a_converter : a_content_serializer_and_an_execution_context
    {
        protected static IConvertEventsToProtobuf converter;

        Establish context = () => converter = new EventToProtobufConverter(serializer.Object);
    }
}

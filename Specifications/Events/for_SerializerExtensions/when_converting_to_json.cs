// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Serialization.Json;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Events.for_SerializerExtensions
{
    public class when_converting_to_json
    {
        static Mock<ISerializer> serializer;

        static MyEvent the_event;

        Establish context = () =>
        {
            the_event = new MyEvent();
            serializer = new Mock<ISerializer>();
        };

        Because of = () => serializer.Object.EventToJson(the_event);

        It should_serialize_with_correct_options = () => serializer.Verify(_ => _.ToJson(the_event, SerializationOptions.CamelCase), Times.Once());
    }
}
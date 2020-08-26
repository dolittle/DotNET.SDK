// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Serialization.Json;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Events.for_SerializerExtensions
{
    public class when_converting_from_json
    {
        static Mock<ISerializer> serializer;

        static string the_event = "{\"something\":42}";

        Establish context = () => serializer = new Mock<ISerializer>();

        Because of = () => serializer.Object.JsonToEvent(typeof(MyEvent), the_event);

        It should_deserialize_with_correct_options = () => serializer.Verify(_ => _.FromJson(typeof(MyEvent), the_event, SerializationOptions.CamelCase), Times.Once());
    }
}
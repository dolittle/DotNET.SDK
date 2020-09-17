// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.SDK.Events.Processing.for_EventProcessingRequestConverter.when_getting_event.given
{
    public class all_dependencies : for_EventProcessingRequestConverter.given.all_dependencies
    {
        protected static EventType event_type;
        protected static an_event @event;
        protected static string event_content;
        Establish context = () =>
        {
            event_type = new EventType(artifact_id, artifact_generation);
            @event = new an_event { a_string = "Hello World", a_bool = true, an_integer = 42 };
            event_content = System.Text.Json.JsonSerializer.Serialize(@event);
            committed_event.Content = @event_content;

            event_types.Setup(_ => _.GetTypeFor(event_type)).Returns(typeof(an_event));
            event_types.Setup(_ => _.GetFor<an_event>()).Returns(event_type);
        };
    }
}

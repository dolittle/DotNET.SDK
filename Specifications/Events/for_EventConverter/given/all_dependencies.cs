// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Artifacts.Contracts;
using Dolittle.Execution.Contracts;
using Dolittle.SDK.Artifacts;
using Google.Protobuf.WellKnownTypes;
using Machine.Specifications;
using Moq;
using Contracts = Dolittle.Runtime.Events.Contracts;

namespace Dolittle.SDK.Events.for_EventConverter.given
{
    public class all_dependencies : a_converter
    {
        protected static Guid artifact_id;
        protected static Generation artifact_generation;
        protected static Artifact artifact;
        protected static EventType event_type;
        protected static an_event @event;
        protected static string event_content;
        protected static CommittedEvent committed_event;

        Establish context = () =>
        {
            artifact_id = Guid.Parse("ef46e5ab-36ee-4d6c-9f44-a94a4d433659");
            artifact_generation = 2;
            // event_type = new EventType(artifact_id, artifact_generation);
            @event = new an_event { a_string = "Hello World", a_bool = true, an_integer = 42 };
            event_content = System.Text.Json.JsonSerializer.Serialize(@event);

            artifact = new Artifact { Id = artifact_id.ToProtobuf(), Generation = artifact_generation };

            committed_event = new Contracts.CommittedEvent
            {
                Content = @event_content,
                EventLogSequenceNumber = 1,
                EventSourceId = Guid.Parse("c541ab5e-282a-4dae-8807-81340c0513ae").ToProtobuf(),
                ExecutionContext = new Mock<ExecutionContext>().Object,
                External = false,
                ExternalEventLogSequenceNumber = 0,
                ExternalEventReceived = null,
                Occurred = Timestamp.FromDateTimeOffset(DateTimeOffset.FromUnixTimeSeconds(14231512312)),
                Public = false,
                Type = artifact
            };

            event_types.Setup(_ => _.GetTypeFor(event_type)).Returns(typeof(an_event));
            event_types.Setup(_ => _.GetFor<an_event>()).Returns(event_type);
        };
    }
}

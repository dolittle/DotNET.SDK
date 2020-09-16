// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using Dolittle.Artifacts.Contracts;
using Dolittle.Execution.Contracts;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.SDK.Artifacts;
using Dolittle.SDK.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Machine.Specifications;

namespace Dolittle.SDK.Events.Processing.for_EventProcessingRequestConverter.given
{
    public class all_dependencies : a_converter
    {
        protected static Guid artifact_id;
        protected static Generation artifact_generation;
        protected static ExecutionContext execution_context;
        protected static Artifact artifact;
        protected static CommittedEvent committed_event;

        Establish context = () =>
        {
            artifact_id = Guid.Parse("ef46e5ab-36ee-4d6c-9f44-a94a4d433659");
            artifact_generation = 2;
            execution_context = new Execution.ExecutionContext(
                "733e2fab-dd41-4d8a-871f-23a7cd5fd185",
                "f9d0c008-ff75-42cc-83a9-24f5857a2b3c",
                Microservices.Version.NotSet,
                "env",
                "a0c5a237-8759-429e-937d-0d671f6f606f",
                Security.Claims.Empty,
                CultureInfo.InvariantCulture).ToProtobuf();

            artifact = new Artifact { Id = artifact_id.ToProtobuf(), Generation = artifact_generation };
            committed_event = new CommittedEvent
            {
                Content = "{}",
                EventLogSequenceNumber = 1,
                EventSourceId = Guid.Parse("c541ab5e-282a-4dae-8807-81340c0513ae").ToProtobuf(),
                ExecutionContext = execution_context,
                External = false,
                ExternalEventLogSequenceNumber = 0,
                ExternalEventReceived = null,
                Occurred = Timestamp.FromDateTimeOffset(DateTimeOffset.FromUnixTimeSeconds(14231512312)),
                Public = false,
                Type = artifact
            };
        };
    }
}

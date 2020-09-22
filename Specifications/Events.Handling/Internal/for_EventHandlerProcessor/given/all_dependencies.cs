// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Linq;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Machine.Specifications;
using Newtonsoft.Json;
using PbCommittedEvent = Dolittle.Runtime.Events.Contracts.CommittedEvent;
using PbStreamEvent = Dolittle.Runtime.Events.Processing.Contracts.StreamEvent;

namespace Dolittle.SDK.Events.Handling.Internal.for_EventHandlerProcessor.given
{
    public class all_dependencies : an_event_handler_processor
    {
        protected static HandleEventRequest request;
        protected static ExecutionContext execution_context;
        protected static PbCommittedEvent committed_event;
        protected static PbStreamEvent stream_event;
        protected static some_event event_to_handle;

        Establish context = () =>
        {
            execution_context = new ExecutionContext(
                "8dd70f32-dcf7-4e05-ac3e-9ea6653040b9",
                "f8b322b1-f5e4-45ad-b5d5-96c020d294c9",
                Microservices.Version.NotSet,
                "an environment",
                "c1da9511-f1af-431e-86e3-69141a391e91",
                Security.Claims.Empty,
                CultureInfo.InvariantCulture);
            event_to_handle = new some_event { some_string = "hello world" };
            committed_event = new PbCommittedEvent
            {
                Content = JsonConvert.SerializeObject(event_to_handle),
                EventLogSequenceNumber = 3,
                EventSourceId = Guid.Parse("bf56ce13-a82a-4ce9-b155-8511e485bdb3").ToProtobuf(),
                External = false,
                ExternalEventLogSequenceNumber = 0,
                ExternalEventReceived = null,
                Occurred = Timestamp.FromDateTimeOffset(DateTimeOffset.Now),
                Public = false,
                Type = handled_event_types.First().ToProtobuf(),
                ExecutionContext = execution_context.ToProtobuf()
            };
            stream_event = new PbStreamEvent
            {
                Event = committed_event,
                Partitioned = partitioned,
                PartitionId = Guid.Parse("19f78b31-47e3-4032-b267-a3fe01509fec").ToProtobuf(),
                ScopeId = event_handler_scope.ToProtobuf()
            };
            request = new HandleEventRequest
            {
                Event = stream_event,
                RetryProcessingState = null
            };
        };
    }
}
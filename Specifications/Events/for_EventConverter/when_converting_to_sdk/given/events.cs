// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using Dolittle.SDK.Protobuf;
using Dolittle.Execution.Contracts;
using Machine.Specifications;
using Newtonsoft.Json;
using Contracts = Dolittle.Runtime.Events.Contracts;

namespace Dolittle.SDK.Events.for_EventConverter.when_converting_to_sdk.given
{
    public class events : for_EventConverter.given.a_converter
    {
        protected static Contracts.CommittedEvent committed_event;
        protected static Contracts.CommittedEvent committed_external_event;
        protected static EventSourceId event_source_id;
        protected static EventType event_type;
        protected static an_event @event;

        Establish context = () =>
        {
            event_source_id = EventSourceId.New();
            event_type = new EventType(Guid.NewGuid());
            @event = new an_event { a_string = "Hello World", a_bool = true, an_integer = 42 };
            var execution_context = new Execution.ExecutionContext(
                "733e2fab-dd41-4d8a-871f-23a7cd5fd185",
                "f9d0c008-ff75-42cc-83a9-24f5857a2b3c",
                Microservices.Version.NotSet,
                "env",
                "a0c5a237-8759-429e-937d-0d671f6f606f",
                Security.Claims.Empty,
                CultureInfo.InvariantCulture).ToProtobuf();

            committed_event = new Contracts.CommittedEvent
                {
                    EventLogSequenceNumber = 1,
                    EventSourceId = event_source_id.ToProtobuf(),
                    ExecutionContext = 

                }
            committed_event = new Contracts.UncommittedEvent
                {
                    Artifact = event_type.ToProtobuf(),
                    EventSourceId = event_source_id.ToProtobuf(),
                    Public = false,
                    Content = JsonConvert.SerializeObject(@event)
                };
            committed_external_event = new Contracts.UncommittedEvent
                {
                    Artifact = event_type.ToProtobuf(),
                    EventSourceId = event_source_id.ToProtobuf(),
                    Public = true,
                    Content = JsonConvert.SerializeObject(@event),
                    
                };
            
            event_types.Setup(_ => _.GetTypeFor(event_type)).Returns(typeof(an_event));
            event_types.Setup(_ => _.GetFor<an_event>()).Returns(event_type);
        };
    }
}

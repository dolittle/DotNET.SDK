// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.SDK.Events.Processing;
using Machine.Specifications;
using Microsoft.Extensions.Logging;
using Moq;

namespace Dolittle.SDK.Events.Handling.Internal.for_EventHandlerProcessor.given;

public class an_event_handler_processor
{
    protected static EventHandlerId event_handler_id;
    protected static ScopeId event_handler_scope;
    protected static bool partitioned;
    protected static IEnumerable<EventType> handled_event_types;
    protected static Mock<IEventHandler> event_handler;
    protected static Mock<IEventTypes> event_types;
    protected static Mock<IEventProcessingConverter> event_processing_converter;
    protected static EventHandlerProcessor event_handler_processor;

    Establish context = () =>
    {
        event_handler = new Mock<IEventHandler>();
        event_types = new Mock<IEventTypes>();
        event_handler_id = "1d6f4e60-6453-423e-baa0-3dda3ecaa719";
        event_handler_scope = "35b14dda-ac5e-4ab2-9bf3-c710ffb17d69";
        partitioned = true;
        handled_event_types =
        [
            new("a57ecc5a-9fca-47a3-89d0-24064a6f9e34"),
            new("3caf8431-d1c7-42a0-92fd-04ca4dcdf77c")
        ];

        event_handler.SetupGet(_ => _.Identifier).Returns(event_handler_id);
        event_handler.SetupGet(_ => _.ScopeId).Returns(event_handler_scope);
        event_handler.SetupGet(_ => _.Partitioned).Returns(partitioned);
        event_handler.SetupGet(_ => _.HandledEvents).Returns(handled_event_types);
        event_processing_converter = new Mock<IEventProcessingConverter>();
        event_handler_processor = new EventHandlerProcessor(event_handler.Object, event_processing_converter.Object, Mock.Of<ILogger>());
    };
}
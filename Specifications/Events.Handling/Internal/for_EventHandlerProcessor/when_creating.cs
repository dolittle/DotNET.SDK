// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.SDK.Events.Processing;
using Dolittle.SDK.Protobuf;
using Machine.Specifications;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Handling.Internal.for_EventHandlerProcessor;

public class when_creating
{
    static EventHandlerModelId event_handler_id;
    static IEnumerable<EventType> handled_event_types;
    static Moq.Mock<IEventHandler> event_handler;
    static Moq.Mock<IEventProcessingConverter> event_processing_converter;
    static EventHandlerProcessor event_handler_processor;

    Establish context = () =>
    {
        event_handler = new Moq.Mock<IEventHandler>();
        event_processing_converter = new Moq.Mock<IEventProcessingConverter>();

        event_handler_id = new EventHandlerModelId("1d6f4e60-6453-423e-baa0-3dda3ecaa719", true, "35b14dda-ac5e-4ab2-9bf3-c710ffb17d69", "alias");
        handled_event_types = new EventType[]
        {
            new("a57ecc5a-9fca-47a3-89d0-24064a6f9e34"),
            new("3caf8431-d1c7-42a0-92fd-04ca4dcdf77c")
        };

        event_handler.SetupGet(_ => _.Identifier).Returns(event_handler_id);
        event_handler.SetupGet(_ => _.HandledEvents).Returns(handled_event_types);
    };

    Because of = () => event_handler_processor = new EventHandlerProcessor(event_handler.Object, event_processing_converter.Object, Moq.Mock.Of<ILogger>());

    It should_not_be_null = () => event_handler_processor.ShouldNotBeNull();
    It should_have_the_correct_identifier = () => event_handler_processor.Identifier.ShouldEqual(event_handler_id.Id);
    It should_have_the_correct_registartion_request = () =>
    {
        var registration_request = event_handler_processor.RegistrationRequest;
        registration_request.EventHandlerId.ShouldEqual(event_handler_id.Id.ToProtobuf());
        registration_request.ScopeId.ShouldEqual(event_handler_id.Scope.ToProtobuf());
        registration_request.Partitioned.ShouldEqual(event_handler_id.Partitioned);
        registration_request.EventTypes.ShouldContainOnly(handled_event_types.Select(_ => _.ToProtobuf()));
    };
}
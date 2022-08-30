// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.SDK.Events.Handling.Builder.Methods;
using Machine.Specifications;
using Moq;

namespace Dolittle.SDK.Events.Handling.for_EventHandler.when_handling.given;

public class an_event_handler
{
    protected static EventHandlerModelId identifier;
    protected static EventType handled_event_type;
    protected static Mock<IEventHandlerMethod> event_handler_method;
    protected static IDictionary<EventType, IEventHandlerMethod> event_handler_methods;
    protected static EventHandler event_handler;

    Establish context = () =>
    {
        identifier = new EventHandlerModelId("e869d29e-e89b-4228-b345-36ec472d9aab", true, "791c9d34-9b0e-4fc2-898d-24fefbf4e9c1", "some alias");
        handled_event_type = new EventType("1b9680f7-92bd-4e1f-ac4b-1aae55944209");
        event_handler_method = new Mock<IEventHandlerMethod>();
        event_handler_methods = new Dictionary<EventType, IEventHandlerMethod>
        {
            { handled_event_type, event_handler_method.Object }
        };
        event_handler = new EventHandler(identifier, event_handler_methods);
    };
}
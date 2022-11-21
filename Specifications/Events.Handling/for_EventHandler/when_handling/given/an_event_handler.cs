// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.SDK.Events.Handling.Builder.Methods;
using Machine.Specifications;
using Moq;

namespace Dolittle.SDK.Events.Handling.for_EventHandler.when_handling.given;

public class an_event_handler
{
    protected static EventHandlerId identifier;
    protected static ScopeId scope_id;
    protected static bool partitioned;
    protected static EventType handled_event_type;
    protected static Mock<IEventHandlerMethod> event_handler_method;
    protected static IDictionary<EventType, IEventHandlerMethod> event_handler_methods;
    protected static EventHandler event_handler;

    Establish context = () =>
    {
        identifier = "e869d29e-e89b-4228-b345-36ec472d9aab";
        scope_id = "455b99e2-7647-4851-b77c-3bee89902ac3";
        partitioned = true;
        handled_event_type = new EventType("1b9680f7-92bd-4e1f-ac4b-1aae55944209");
        event_handler_method = new Mock<IEventHandlerMethod>();
        event_handler_methods = new Dictionary<EventType, IEventHandlerMethod>
        {
            { handled_event_type, event_handler_method.Object }
        };
        event_handler = new EventHandler(new EventHandlerModelId(identifier, partitioned, scope_id, "some alias"), event_handler_methods);
    };
}
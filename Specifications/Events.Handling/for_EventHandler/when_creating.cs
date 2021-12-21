// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.SDK.Events.Handling.Builder;
using Dolittle.SDK.Events.Handling.Builder.Methods;
using Machine.Specifications;

namespace Dolittle.SDK.Events.Handling.for_EventHandler
{
    public class when_creating
    {
        static EventHandlerId identifier;
        static ScopeId scope_id;
        static bool partitioned;
        static IEnumerable<EventType> event_types;
        static IDictionary<EventType, IEventHandlerMethod> event_handler_methods;
        static EventHandler event_handler;

        Establish context = () =>
        {
            identifier = "e869d29e-e89b-4228-b345-36ec472d9aab";
            scope_id = "455b99e2-7647-4851-b77c-3bee89902ac3";
            partitioned = true;
            event_types = new[]
                {
                    new EventType("1b9680f7-92bd-4e1f-ac4b-1aae55944209"),
                    new EventType("27a6d373-1ae6-42ed-a149-e0d69e8aac7c")
                };
            event_handler_methods = new Dictionary<EventType, IEventHandlerMethod>(
                event_types.ToDictionary(
                    _ => _,
                    _ => new EventHandlerMethod((@event, eventContext) => Task.CompletedTask) as IEventHandlerMethod));
        };

        Because of = () => event_handler = new EventHandler(identifier, scope_id, partitioned, event_handler_methods);

        It should_not_be_null = () => event_handler.ShouldNotBeNull();
        It should_have_the_correct_partitioned_value = () => event_handler.Partitioned.ShouldEqual(partitioned);
        It should_have_the_correct_identifier = () => event_handler.Identifier.ShouldEqual(identifier);
        It should_have_the_correct_scope_id = () => event_handler.ScopeId.ShouldEqual(scope_id);
        It should_handle_only_the_correct_events = () => event_handler.HandledEvents.ShouldContainOnly(event_types);
    }
}

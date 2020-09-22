// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Services;
using Machine.Specifications;
using Microsoft.Extensions.Logging;
using Moq;

namespace Dolittle.SDK.Events.for_EventStore.given
{
    public class an_event_store_and_an_execution_context : an_execution_context
    {
        protected static IPerformMethodCalls caller;
        protected static IEventConverter converter;
        protected static Mock<IEventTypes> event_types;
        protected static IEventStore event_store;

        Establish context = () =>
        {
            caller = new MethodCaller("localhost", 50055);
            event_types = new Mock<IEventTypes>();
            converter = new EventConverter(event_types.Object);

            event_store = new EventStore(caller, converter, execution_context, event_types.Object, Mock.Of<ILogger>());
        };
    }
}

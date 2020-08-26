// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Events.Handling.Internal;
using Dolittle.Events.Processing.Internal;
using Dolittle.Resilience;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;
using MIt = Moq.It;

namespace Dolittle.Events.Handling.for_EventHandlerRegistry
{
    public class when_registering_an_event_handler
    {
        static Mock<IAsyncPolicyFor<EventHandlerRegistry>> _policy;
        static Mock<IEventProcessingCompletion> _completion;

        static Mock<IEventHandler<IEvent>> _handler;

        static Mock<IEventHandlerProcessors> _processors;

        static Mock<IEventProcessor> _processor;

        static EventHandlerRegistry registry;

        Establish context = () =>
        {
            _policy = new Mock<IAsyncPolicyFor<EventHandlerRegistry>>();
            _completion = new Mock<IEventProcessingCompletion>();
            _handler = new Mock<IEventHandler<IEvent>>();
            _processor = new Mock<IEventProcessor>();

            _processors = new Mock<IEventHandlerProcessors>();
            _processors.Setup(_ => _.GetFor<IEvent>(
                MIt.IsAny<EventHandlerId>(),
                MIt.IsAny<ScopeId>(),
                MIt.IsAny<bool>(),
                MIt.IsAny<IEventHandler<IEvent>>()))
                .Returns(_processor.Object);
            registry = new EventHandlerRegistry(_processors.Object, _policy.Object, _completion.Object);
        };

        Because of = () => registry.Register<IEvent>(Guid.Empty, Guid.Empty, false, _handler.Object);

        It should_register_the_handler_to_the_completion = () =>
            _completion.Verify(_ => _.RegisterHandler(Guid.Empty, _handler.Object.HandledEventTypes), Times.Once());

        It should_get_a_processor_from_processors = () =>
            _processors.Verify(_ => _.GetFor(Guid.Empty, Guid.Empty, false, _handler.Object), Times.Once());

        It should_register_a_processor_with_policy = () =>
            _processor.Verify(_ => _.RegisterAndHandleForeverWithPolicy(_policy.Object, default), Times.Once());
    }
}

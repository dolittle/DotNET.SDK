// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Async;
using Machine.Specifications;

namespace Dolittle.SDK.Events.Handling.for_EventHandler.when_handling
{
    public class an_event : given.all_dependencies
    {
        static Exception exception;

        Establish context = ()
            => event_handler_method.Setup(_ => _.TryHandle(Moq.It.IsAny<object>(), Moq.It.IsAny<EventContext>())).Returns(Task.FromResult(new Try()));

        Because of = () => exception = Catch.Exception(() => event_handler.Handle(@event, handled_event_type, event_context, CancellationToken.None).GetAwaiter().GetResult());

        It should_invoke_handler_with_correct_arguments = () => event_handler_method.Verify(_ => _.TryHandle(@event, event_context), Moq.Times.Once);
        It should_not_throw_exception = () => exception.ShouldBeNull();
    }
}

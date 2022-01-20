// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Async;
using Machine.Specifications;

namespace Dolittle.SDK.Events.Handling.for_EventHandler.when_handling;

public class and_handler_method_fails : given.all_dependencies
{
    static Exception thrown_exception;
    static Exception exception;

    Establish context = () =>
    {
        thrown_exception = new Exception("Something went wrong");
        event_handler_method
            .Setup(_ => _.TryHandle(Moq.It.IsAny<object>(), Moq.It.IsAny<EventContext>(), Moq.It.IsAny<IServiceProvider>()))
            .Returns(Task.FromResult<Try>(thrown_exception));
    };

    Because of = () => exception = Catch.Exception(() => event_handler.Handle(@event, handled_event_type, event_context, service_provider.Object, CancellationToken.None).GetAwaiter().GetResult());

    It should_invoke_handler_with_correct_arguments = () => event_handler_method.Verify(_ => _.TryHandle(@event, event_context, service_provider.Object), Moq.Times.Once);
    It should_throw_exception_because_event_handler_method_failed = () => exception.ShouldBeOfExactType<EventHandlerMethodFailed>();
    It should_throw_exception_that_contains_the_original_thrown_exception = () => exception.InnerException.ShouldEqual(thrown_exception);
}
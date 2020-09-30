// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.SDK.EventHorizon.for_SubscriptionCallbacks
{
    public class when_calling_on_error : given.a_subscription_callbacks_with_handlers
    {
        static Exception exception;

        Establish context = () => exception = new Exception("Something went wrong");

        Because of = () => subscription_callbacks.OnError(exception);

        It should_not_invoke_on_success = () => succeeded_handler.VerifyNoOtherCalls();
        It should_not_invoke_on_failure = () => failed_handler.VerifyNoOtherCalls();
        It should_not_invoke_on_completed = () => completed_handler.VerifyNoOtherCalls();
    }
}
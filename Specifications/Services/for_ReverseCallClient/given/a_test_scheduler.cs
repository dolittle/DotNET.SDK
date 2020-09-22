// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using Microsoft.Reactive.Testing;

namespace Dolittle.SDK.Services.for_ReverseCallClient.given
{
    public class a_test_scheduler : ReactiveTest
    {
        protected static TestScheduler scheduler;

        Establish context = () => scheduler = new TestScheduler();
    }
}
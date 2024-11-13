// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Globalization;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Security;
using Machine.Specifications;

namespace Dolittle.SDK.Events.Store.for_EventStore.given;

public class an_execution_context
{
    protected static ExecutionContext execution_context;

    Establish context = () =>
    {
        execution_context = new ExecutionContext(
            "388ed676-2448-45fe-99c7-93094e8241c4",
            "6714619f-b2dd-4d90-a545-642df1bbb67b",
            new Microservices.Version(5, 4, 3, 2, "duvorwuicu"),
            "latmazokcu",
            "a6d990c8-81cb-4a8b-b7cf-327c83ce7cc4",
            new Claims([
                new Claim("bibetudicu", "efajeakuga", "wovawjohoa")
            ]),
            CultureInfo.InvariantCulture,
            ActivitySpanId.CreateFromString("b00lb00lb00lb00l")
            );
    };
}
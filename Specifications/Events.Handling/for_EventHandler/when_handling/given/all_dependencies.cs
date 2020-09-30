// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using Machine.Specifications;

namespace Dolittle.SDK.Events.Handling.for_EventHandler.when_handling.given
{
    public class all_dependencies : an_event_handler
    {
        protected static EventContext event_context;
        protected static object @event;

        Establish context = () =>
        {
            event_context = new EventContext(
                3,
                "7d387632-68d8-4187-aa8a-b7d191a1b130",
                DateTimeOffset.UtcNow,
                new Execution.ExecutionContext(
                    "98bcb77f-4259-4038-a1ec-a9a7b0b65d93",
                    "d74a685d-4bc7-4dd3-a168-20e4da0bf344",
                    Microservices.Version.NotSet,
                    "some environment",
                    "281beded-b881-4f8c-a78e-260a15497adf",
                    Security.Claims.Empty,
                    CultureInfo.InvariantCulture),
                new Execution.ExecutionContext(
                    "d4810912-cb58-43d4-b3a2-f8b8a3530d64",
                    "dcc97ef3-0165-4881-9f3d-d7368b7a47b0",
                    Microservices.Version.NotSet,
                    "some environment",
                    "1451deb6-59e8-4f11-8c8e-88884e1fb7a8",
                    Security.Claims.Empty,
                    CultureInfo.InvariantCulture));
            @event = new object();
        };
    }
}
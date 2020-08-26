// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using Dolittle.Execution;
using Dolittle.Security;
using Version = Dolittle.Versioning.Version;

namespace Dolittle.Events.given
{
    public abstract class Events
    {
        public static readonly ExecutionContext execution_context = new ExecutionContext(
            Guid.Parse("f251f016-d622-4da4-9b1f-7e078cc89c80"),
            Guid.Parse("be17a469-edd4-4b0d-bc2b-206d0b5e69f0"),
            new Version(1, 1, 1, 1, "alpha.1"),
            "Environment",
            Guid.Parse("1e346956-b2bf-4b4b-a4d9-31da532c8bf6"),
            new Claims(Array.Empty<Claim>()),
            CultureInfo.InvariantCulture);

        public static readonly SimpleEvent event_one = new SimpleEvent { Content = "One" };

        public static readonly SimpleEvent event_two = new SimpleEvent { Content = "Two" };

        public static readonly AnotherEvent event_three = new AnotherEvent { Content = "Two" };
    }
}
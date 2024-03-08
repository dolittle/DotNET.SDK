// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using Dolittle.SDK.Events;
using Dolittle.SDK.Security;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.Projections;

public static class TestEventContexts
{
    public static EventContext ForType<T>() where T : class
    {
        var eventType = EventTypeMetadata<T>.EventType;
        return new EventContext(EventLogSequenceNumber.Initial, eventType!, EventSourceId.New(), DateTimeOffset.Now, CreateExecutionContext(),
            CreateExecutionContext());
    }

    static ExecutionContext CreateExecutionContext()
    {
        return new ExecutionContext(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Dolittle.SDK.Microservices.Version.NotSet,
            "env",
            Guid.NewGuid(),
            Claims.Empty,
            CultureInfo.InvariantCulture);
    }
}

using System;
using System.Globalization;
using Dolittle.SDK.Common.Model;
using Dolittle.SDK.Events;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Security;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;
using Version = Dolittle.SDK.Microservices.Version;

namespace Specs;

class given
{
    internal static EventContext an_event_context_for<TEvent>(EventSourceId eventSource, EventLogSequenceNumber sequenceNumber = null)
    {
        if (!typeof(TEvent).TryGetIdentifier(out EventType eventType))
        {
            throw new ArgumentException($"{typeof(TEvent)} is not decorated with {nameof(EventTypeAttribute)} attribute");
        }
        return new EventContext(
            sequenceNumber ?? 0,
            eventType,
            eventSource,
            DateTimeOffset.Now,
            new ExecutionContext(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Version.NotSet,
                "env",
                Guid.NewGuid(),
                Claims.Empty,
                CultureInfo.InvariantCulture),
            new ExecutionContext(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Version.NotSet,
                "env",
                Guid.NewGuid(),
                Claims.Empty,
                CultureInfo.InvariantCulture)
        );
    }
}

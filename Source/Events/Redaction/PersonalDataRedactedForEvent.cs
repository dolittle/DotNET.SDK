// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Dolittle.SDK.Events.Redaction;

/// <summary>
/// Event that triggers redaction of the given personal data
/// It will target the given event type and redact the properties specified within the EventSourceId of the event
/// </summary>
[EventType("de1e7e17-bad5-da7a-fad4-fbc6ec3c0ea5")]
public class PersonalDataRedactedForEvent
{
    public string EventId { get; init; }
    public string EventAlias { get; init; }
    public Dictionary<string, object?> RedactedProperties { get; init; }
    public string RedactedBy { get; init; }
    public string Reason { get; init; }

    /// <summary>
    /// Try to create a PersonalDataRedacted event for a given event type
    /// Must include reason and redactedBy
    /// </summary>
    /// <param name="reason"></param>
    /// <param name="redactedBy"></param>
    /// <param name="redactionEvent"></param>
    /// <param name="error"></param>
    /// <typeparam name="TEvent"></typeparam>
    /// <returns></returns>
    public static bool TryCreate<TEvent>(string reason, string redactedBy,
        [NotNullWhen(true)] out PersonalDataRedactedForEvent? redactionEvent, [NotNullWhen(false)] out string? error)
        where TEvent : class
    {
        redactionEvent = default;
        error = default;
        if (string.IsNullOrWhiteSpace(reason))
        {
            error = "Reason cannot be empty";
            return false;
        }

        if (string.IsNullOrWhiteSpace(redactedBy))
        {
            error = "RedactedBy cannot be empty";
            return false;
        }

        var eventType = EventTypeMetadata<TEvent>.EventType;
        if (eventType?.Id is null)
        {
            error = $"EventType not defined for type {typeof(TEvent).Name}";
            return false;
        }

        var redactedFields = RedactedType<TEvent>.RedactedProperties;
        if (redactedFields.Count == 0)
        {
            error = $"No redacted fields found for event type {eventType.Alias}";
            return false;
        }

        redactionEvent = new PersonalDataRedactedForEvent
        {
            EventId = eventType.Id.ToString(),
            EventAlias = eventType.Alias ?? typeof(TEvent).Name,
            RedactedProperties = new Dictionary<string, object?>(redactedFields),
            RedactedBy = redactedBy,
            Reason = reason
        };
        return true;
    }
}

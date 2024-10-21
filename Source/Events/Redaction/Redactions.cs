// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Dolittle.SDK.Events.Redaction;

public static class Redactions
{
    /// <summary>
    /// This is recognized by the runtime, and triggers redaction of the selected personal data
    /// </summary>
    public const string PersonalDataRedactedPrefix = "de1e7e17-bad5-da7a";
    public const string PersonalDataRedactedId = "de1e7e17-bad5-da7a-fad4-fbc6ec3c0ea5";


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
        where TEvent : class =>
        InternalTryCreate<TEvent, PersonalDataRedactedForEvent>(reason, redactedBy, out redactionEvent, out error);

    
    /// <summary>
    /// Try to create a PersonalDataRedacted event for a given event type
    /// Must include reason and redactedBy
    /// </summary>
    /// <param name="reason"></param>
    /// <param name="redactedBy"></param>
    /// <param name="redactionEvent"></param>
    /// <param name="error"></param>
    /// <typeparam name="TEvent">The redacted event</typeparam>
    /// <typeparam name="TRedactionEvent">The redaction event type</typeparam>
    /// <returns></returns>
    public static bool TryCreate<TEvent, TRedactionEvent>(string reason, string redactedBy,
        [NotNullWhen(true)] out TRedactionEvent? redactionEvent, [NotNullWhen(false)] out string? error)
        where TEvent : class
        where TRedactionEvent : PersonalDataRedactedForEvent<TEvent>, new() =>
    InternalTryCreate<TEvent, TRedactionEvent>(reason, redactedBy, out redactionEvent, out error);

    

    /// <summary>
    /// Try to create a PersonalDataRedacted event for a given event type
    /// Must include reason and redactedBy
    /// </summary>
    /// <param name="reason"></param>
    /// <param name="redactedBy"></param>
    /// <param name="redactionEvent"></param>
    /// <param name="error"></param>
    /// <typeparam name="TEvent">The redacted event</typeparam>
    /// <typeparam name="TRedactionEvent">The redaction event type</typeparam>
    /// <returns></returns>
    static bool InternalTryCreate<TEvent, TRedactionEvent>(string reason, string redactedBy,
        [NotNullWhen(true)] out TRedactionEvent? redactionEvent, [NotNullWhen(false)] out string? error)
        where TEvent : class
        where TRedactionEvent : PersonalDataRedacted, new()
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

        var idStr = eventType.Id.ToString();
        if (idStr.StartsWith(PersonalDataRedactedPrefix, StringComparison.InvariantCultureIgnoreCase))
        {
            error = "Cannot create redaction event for redaction event";
            return false;
        }

        var redactedFields = RedactedType<TEvent>.RedactedProperties;
        if (redactedFields.Count == 0)
        {
            error = $"No redacted fields found for event type {eventType.Alias}";
            return false;
        }

        redactionEvent = new TRedactionEvent
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

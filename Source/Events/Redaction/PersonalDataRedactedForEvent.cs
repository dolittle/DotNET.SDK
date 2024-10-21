// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Dolittle.SDK.Events.Redaction;

/// <summary>
/// Event that triggers redaction of the given personal data
/// It will target the given event type and redact the properties specified within the EventSourceId of the event
/// </summary>
[EventType(Redactions.PersonalDataRedactedId)]
public abstract class PersonalDataRedacted
{
    public string EventId { get; init; }
    public string EventAlias { get; init; }

    /// <summary>
    /// The properties that will be redacted, and the replacement values.
    /// Can be null, in which case the properties will be redacted with a default value
    /// </summary>
    public Dictionary<string, object?> RedactedProperties { get; init; }

    public string RedactedBy { get; init; }
    public string Reason { get; init; }
}

/// <summary>
/// Event that triggers redaction of the given personal data
/// This is a built-in event type that is recognized by the runtime
/// </summary>
[EventType(Redactions.PersonalDataRedactedId)]
public class 
    PersonalDataRedactedForEvent : PersonalDataRedacted;

/// <summary>
/// Extend this type to create a redaction event for a specific event type
/// Events of this type will be recognized by the runtime and trigger redaction of the selected personal data
/// They must use the event type id prefix Redactions.PersonalDataRedactedPrefix (de1e7e17-bad5-da7a)
/// </summary>
/// <typeparam name="TEvent"></typeparam>
public abstract class PersonalDataRedactedForEvent<TEvent> : PersonalDataRedacted where TEvent: class;

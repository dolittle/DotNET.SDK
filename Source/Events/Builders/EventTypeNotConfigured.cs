// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Events.Builders;

/// <summary>
/// Exception that gets thrown when building an event and there is no <see cref="EventType" /> configured for the event.
/// </summary>
public class EventTypeNotConfigured : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EventTypeNotConfigured"/> class.
    /// </summary>
    /// <param name="typeOfEvent">The <see cref="Type" /> of the event.</param>
    public EventTypeNotConfigured(Type typeOfEvent)
        : base($"Could not build event because no event type was configured and {typeOfEvent} is not decorated with [{nameof(EventTypeAttribute)}]. Call {nameof(EventBuilder.WithEventType)}() on event builder or decorate the type of the event with [{nameof(EventTypeAttribute)}]")
    {
    }
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Events.Builders;

/// <summary>
/// Exception that gets thrown when the <see cref="EventBuilder" /> is attempting to build an event that is configured with
/// an <see cref="EventType" /> that does not match the associated <see cref="EventType" />.
/// </summary>
public class ConfiguredEventTypeDoesNotMatchAssociatedEventType : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConfiguredEventTypeDoesNotMatchAssociatedEventType"/> class.
    /// </summary>
    /// <param name="eventType">The configured <see cref="EventType" />.</param>
    /// <param name="associatedEventType">The associated <see cref="EventType" />.</param>
    public ConfiguredEventTypeDoesNotMatchAssociatedEventType(EventType eventType, EventType associatedEventType)
        : base($"Failed to build event because the configured event type {eventType} does not match with the associated event type {associatedEventType}")
    {
    }
}
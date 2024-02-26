// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Events.Handling;

/// <summary>
/// Exception that gets thrown when an event handler method failed handling an event.
/// </summary>
public class EventHandlerMethodFailed : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EventHandlerMethodFailed"/> class.
    /// </summary>
    /// <param name="eventHandlerId">The <see cref="EventHandlerId" />.</param>
    /// <param name="eventType">The <see cref="EventType" />.</param>
    /// <param name="event">The event that failed handling.</param>
    /// <param name="exception">The <see cref="Exception" /> that caused the handling to fail.</param>
    public EventHandlerMethodFailed(EventHandlerId eventHandlerId, EventType eventType, object @event, Exception exception)
        : base($"Event handler {eventHandlerId} failed to handle event {@event} with event type {eventType} ", exception)
    {
    }
}

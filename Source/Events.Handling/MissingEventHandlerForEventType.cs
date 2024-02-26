// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Events.Handling;

/// <summary>
/// Exception that gets thrown when there is no event handler for a specific event type.
/// </summary>
public class MissingEventHandlerForEventType : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MissingEventHandlerForEventType"/> class.
    /// </summary>
    /// <param name="eventType">The <see cref="EventType" />.</param>
    public MissingEventHandlerForEventType(EventType eventType)
        : base($"Missing event handler for '{eventType}'")
    {
    }
}

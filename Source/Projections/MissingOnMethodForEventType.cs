// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections;

/// <summary>
/// Exception that gets thrown when there is no on-method for a specific event type.
/// </summary>
public class MissingOnMethodForEventType : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MissingOnMethodForEventType"/> class.
    /// </summary>
    /// <param name="eventType">The <see cref="EventType" />.</param>
    public MissingOnMethodForEventType(EventType eventType)
        : base($"Missing on-method for '{eventType}'")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MissingOnMethodForEventType"/> class.
    /// </summary>
    /// <param name="eventType">The <see cref="EventType" />.</param>
    /// <param name="projectionType">The projection type</param>
    public MissingOnMethodForEventType(Type eventType, Type projectionType)
        : base($"Missing on-method for '{eventType.FullName}' in projection '{projectionType.FullName}'")
    {
    }
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Events;

/// <summary>
/// Exception that gets thrown when a <see cref="EventType" /> does not have an <see cref="Type"/> association.
/// </summary>
public class NoTypeAssociatedWithEventType : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NoTypeAssociatedWithEventType"/> class.
    /// </summary>
    /// <param name="eventType">The <see cref="EventType" /> that has a missing association.</param>
    public NoTypeAssociatedWithEventType(EventType eventType)
        : base($"{eventType} is not associated with a Type")
    {
    }
}
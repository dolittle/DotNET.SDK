// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Artifacts;

namespace Dolittle.SDK.Events;

/// <summary>
/// Exception that gets thrown when trying to construct an <see cref="EventType"/> with a <see cref="Generation"/> that is null.
/// </summary>
public class EventTypeGenerationCannotBeNull : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EventTypeGenerationCannotBeNull"/> class.
    /// </summary>
    public EventTypeGenerationCannotBeNull()
        : base($"The {nameof(Generation)} of an {nameof(EventType)} cannot be null")
    {
        
    }
}

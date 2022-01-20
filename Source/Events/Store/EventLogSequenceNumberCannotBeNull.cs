// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Events.Store;

/// <summary>
/// Exception that gets thrown when trying to construct a <see cref="CommittedEvent"/> without an <see cref="EventLogSequenceNumber"/>.
/// </summary>
public class EventLogSequenceNumberCannotBeNull : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EventLogSequenceNumberCannotBeNull"/> class.
    /// </summary>
    public EventLogSequenceNumberCannotBeNull()
        : base($"The {nameof(EventLogSequenceNumber)} of an {nameof(CommittedEvent)} cannot be null")
    {
    }
}
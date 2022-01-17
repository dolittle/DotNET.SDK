// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Failures.Events;

/// <summary>
/// Exception that gets thrown when an event is being used with an Event Source with a different event source id than it was applied to.
/// </summary>
public class EventWasAppliedToOtherEventSource : ArgumentException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EventWasAppliedToOtherEventSource"/> class.
    /// </summary>
    /// <param name="reason">The failure reason.</param>
    public EventWasAppliedToOtherEventSource(string reason)
        : base(reason)
    {
    }
}
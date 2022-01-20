// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Failures.Events;

/// <summary>
/// Exception that gets thrown when the Event Store is unavailable.
/// </summary>
public class EventStoreUnavailable : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreUnavailable"/> class.
    /// </summary>
    /// <param name="reason">The failure reason.</param>
    public EventStoreUnavailable(string reason)
        : base(reason)
    {
    }
}
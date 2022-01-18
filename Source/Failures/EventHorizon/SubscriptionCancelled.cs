// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Failures.EventHorizon;

/// <summary>
/// Exception that gets thrown when an event horizon subscription is cancelled.
/// </summary>
public class SubscriptionCancelled : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SubscriptionCancelled"/> class.
    /// </summary>
    /// <param name="reason">The failure reason.</param>
    public SubscriptionCancelled(string reason)
        : base(reason)
    {
    }
}
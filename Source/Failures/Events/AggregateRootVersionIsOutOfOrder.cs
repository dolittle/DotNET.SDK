// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Failures.Events;

/// <summary>
/// Exception that gets thrown when a sequence of events are not valid for the Aggregate Root it is being used with.
/// </summary>s
public class AggregateRootVersionIsOutOfOrder : ArgumentException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRootVersionIsOutOfOrder"/> class.
    /// </summary>
    /// <param name="reason">The failure reason.</param>
    public AggregateRootVersionIsOutOfOrder(string reason)
        : base(reason)
    {
    }
}

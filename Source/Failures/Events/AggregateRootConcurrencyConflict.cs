// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Failures.Events;

/// <summary>
/// Exception that gets thrown when there is a concurrency conflict in an Aggregate Root for a specific Event Source.
/// </summary>
public class AggregateRootConcurrencyConflict : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRootConcurrencyConflict"/> class.
    /// </summary>
    /// <param name="reason">The failure reason.</param>
    public AggregateRootConcurrencyConflict(string reason)
        : base(reason)
    {
    }
}

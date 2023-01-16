// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Testing.Aggregates;

/// <summary>
/// Exception that gets thrown when there is a Dolittle related assertion that failed.
/// </summary>
public class AggregateAssertionFailed : DolittleAssertionFailed
{
#pragma warning disable CS0618
    public static void Throw(AggregateRoot aggregateRoot, string reason) => throw new AggregateAssertionFailed(aggregateRoot.GetType(), aggregateRoot.EventSourceId, reason);
#pragma warning restore CS0618
    /// <summary>
    /// Initializes a new instance of the <see cref="DolittleAssertionFailed"/> class.
    /// </summary>
    /// <param name="aggregateRootType">The aggregate root <see cref="Type"/> of the aggregate that failed assertion.</param>
    /// <param name="eventSourceId">The aggregate <see cref="EventSourceId"/> of the aggregate that failed assertion</param>
    /// <param name="reason">The reason why assertion failed.</param>
    public AggregateAssertionFailed(Type aggregateRootType, EventSourceId eventSourceId, string reason)
        : base($"Assertion failed for {aggregateRootType} with Event Source ID {eventSourceId} because {reason}")
    {
    }
}

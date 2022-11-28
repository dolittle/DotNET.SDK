// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Aggregates;

/// <summary>
/// Exception that gets thrown when trying to get the <see cref="EventSourceId"/> for an <see cref="AggregateRoot"/> where the value has not been set yet.
/// </summary>
public class EventSourceIdOnAggregateRootNotReady : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EventSourceIdOnAggregateRootNotReady"/> class.
    /// </summary>
    /// <param name="aggregateRootType">The <see cref="Type"/> of the aggregate root.</param>
#pragma warning disable CS0618
    public EventSourceIdOnAggregateRootNotReady(Type aggregateRootType)
        : base($"Event Source has not yet been set on the {aggregateRootType} aggregate root instance. This typically happens when trying to use the {nameof(AggregateRoot.EventSourceId)} property in the constructor." +
            $" If this is important then all you need to do is to include in the public constructor a parameter with the {typeof(EventSourceId)} type and use that in the bast constructor, then the {nameof(AggregateRoot.EventSourceId)} property will be accessible in the constructor.")
    {
    }
}
#pragma warning restore CS0618

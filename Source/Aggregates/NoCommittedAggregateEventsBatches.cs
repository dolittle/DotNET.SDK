// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Aggregates;

/// <summary>
/// Exception that gets thrown when the fetched aggregate root event stream has no batches.
/// </summary>
public class NoCommittedAggregateEventsBatches : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NoCommittedAggregateEventsBatches"/> class.
    /// </summary>
    /// <param name="aggregateRootId">The aggregate root id.</param>
    /// <param name="eventSourceId">The event source id.</param>
    public NoCommittedAggregateEventsBatches(AggregateRootId aggregateRootId, EventSourceId eventSourceId)
        : base($"No batches of committed aggregate events were received when fetching for aggregate root '{aggregateRootId}' with event source '{eventSourceId}'")
    {
    }
}

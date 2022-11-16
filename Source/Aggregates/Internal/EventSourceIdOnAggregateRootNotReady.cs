// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Aggregates.Internal;

/// <summary>
/// Exception that gets thrown when trying to change the <see cref="EventSourceId"/> for an <see cref="AggregateRoot"/>.
/// </summary>
public class CannotChangeEventSourceIdForAggregateRoot : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CannotChangeEventSourceIdForAggregateRoot"/> class.
    /// </summary>
    /// <param name="aggregateRootType">The <see cref="Type"/> of the aggregate root.</param>
    /// <param name="eventSourceId">The original <see cref="EventSourceId"/>.</param>
    /// <param name="newEventSourceId">The new <see cref="EventSourceId"/>.</param>
    public CannotChangeEventSourceIdForAggregateRoot(Type aggregateRootType, EventSourceId eventSourceId, EventSourceId newEventSourceId)
        : base($"Internal error: Cannot change Event Source on the {aggregateRootType} aggregate root instance from {eventSourceId} to {newEventSourceId}. Changing event source id should not happen and is likely due to a mistake.")
    {}
}

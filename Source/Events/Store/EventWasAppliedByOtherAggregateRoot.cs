// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Events.Store;

/// <summary>
/// Exception that gets thrown when an event is being used with an Aggregate Root with a different
/// <see cref="AggregateRootId"/> than it was applied by.
/// </summary>
public class EventWasAppliedByOtherAggregateRoot : ArgumentException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EventWasAppliedByOtherAggregateRoot"/> class.
    /// </summary>
    /// <param name="eventAggregateRootId">The <see cref="AggregateRootId"/> the event was applied by.</param>
    /// <param name="expectedAggregateRootId"><see cref="AggregateRootId"/> of the Aggregate Root.</param>
    public EventWasAppliedByOtherAggregateRoot(AggregateRootId eventAggregateRootId, AggregateRootId expectedAggregateRootId)
        : base($"Aggregate Root '{eventAggregateRootId}' from event does not match with expected '{expectedAggregateRootId}'.")
    {
    }
}
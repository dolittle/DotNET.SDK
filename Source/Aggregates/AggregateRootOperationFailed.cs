// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Aggregates;

/// <summary>
/// Exception that gets thrown when an <see cref="AggregateRootOperations{TAggregate}.Perform(System.Action{TAggregate},System.Threading.CancellationToken)"/> failed. 
/// </summary>
public class AggregateRootOperationFailed : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRootOperationFailed"/> class.
    /// </summary>
    /// <param name="aggregateRootType">The <see cref="Type"/> of the <see cref="AggregateRoot"/>.</param>
    /// <param name="eventSource">The <see cref="EventSourceId"/> of the aggregate.</param>
    /// <param name="error">The inner <see cref="Exception"/> reason for failure.</param>
    public AggregateRootOperationFailed(Type aggregateRootType, EventSourceId eventSource, Exception error)
        : base($"Failed to perform operation on {aggregateRootType} aggregate with event source {eventSource}", error)
    {
    }
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Aggregates
{
    /// <summary>
    /// Exception that gets thrown when <see cref="IAggregateRootOperations{TAggregate}.Perform(Action{TAggregate})" /> is called twice
    /// on the same instance of <see cref="IAggregateRootOperations{TAggregate}" />.
    /// </summary>
    public class AggregateRootOperationAlreadyPerformed : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRootOperationAlreadyPerformed"/> class.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> of the aggregate root.</param>
        /// <param name="aggregateRootId">The <see cref="AggregateRootId" />.</param>
        /// <param name="eventSourceId">The <see cref="EventSourceId" />.</param>
        public AggregateRootOperationAlreadyPerformed(Type type, AggregateRootId aggregateRootId, EventSourceId eventSourceId)
            : base($"Perform called twice on aggregate root {type} with id {aggregateRootId} and event source id {eventSourceId}")
        {
        }
    }
}
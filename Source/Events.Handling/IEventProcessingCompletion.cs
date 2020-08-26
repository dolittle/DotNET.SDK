// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dolittle.Execution;

namespace Dolittle.Events.Handling
{
    /// <summary>
    /// Defines an interface for waiting for all event handlers to be done.
    /// </summary>
    public interface IEventProcessingCompletion
    {
        /// <summary>
        /// Register an event handler for waiting when appropriate.
        /// </summary>
        /// <param name="eventHandler"><see cref="EventHandlerId"/> to register.</param>
        /// <param name="eventTypes">The types of events the event handler handles.</param>
        void RegisterHandler(EventHandlerId eventHandler, IEnumerable<Type> eventTypes);

        /// <summary>
        /// Signal that a specific event handler with a given Id is done handling a specific <see cref="IEvent"/>.
        /// </summary>
        /// <param name="correlationId"><see cref="CorrelationId"/> in which the event is handled in context of.</param>
        /// <param name="eventHandlerId"><see cref="EventHandlerId"/> that was done handling.</param>
        /// <param name="eventType"><see cref="Type"/> of event that was handled.</param>
        void EventHandlerCompletedForEvent(CorrelationId correlationId, EventHandlerId eventHandlerId, Type eventType);

        /// <summary>
        /// Perform an <see cref="Action"/> and wait till all processing is done.
        /// </summary>
        /// <param name="correlationId"><see cref="CorrelationId"/> to wait for.</param>
        /// <param name="events"><see cref="IEnumerable{T}"/> of <see cref="IEvent">events</see> to wait for.</param>
        /// <param name="action"><see cref="Action"/> to perform.</param>
        /// <returns>Task to wait on.</returns>
        Task Perform(CorrelationId correlationId, IEnumerable<IEvent> events, Action action);
    }
}
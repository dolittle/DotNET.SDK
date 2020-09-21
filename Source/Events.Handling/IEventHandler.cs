// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.Events.Handling
{
    /// <summary>
    /// Defines an event handler.
    /// </summary>
    public interface IEventHandler
    {
        /// <summary>
        /// Gets the unique identifier for event handler - <see cref="EventHandlerId" />.
        /// </summary>
        EventHandlerId Identifier { get; }

        /// <summary>
        /// Gets the scope the event handler is in.
        /// </summary>
        ScopeId ScopeId { get; }

        /// <summary>
        /// Gets a value indicating whether or not the event handler is partitioned.
        /// </summary>
        bool Partitioned { get; }

        /// <summary>
        /// Gets the event types identified by its artifact that is handled by this event handler.
        /// </summary>
        IEnumerable<EventType> HandledEvents { get; }

        /// <summary>
        /// Handle an event.
        /// </summary>
        /// <param name="event">The event to handle.</param>
        /// <param name="eventType">The artifact representign the event type.</param>
        /// <param name="context">The context in which the event is in.</param>
        /// <param name="executionContext">The execution context to handle the request in.</param>
        /// <param name="cancellation">The <see cref="CancellationToken" /> used to cancel the processing of the request.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous action.</returns>
        Task Handle(object @event, EventType eventType, EventContext context, ExecutionContext executionContext, CancellationToken cancellation);
    }
}

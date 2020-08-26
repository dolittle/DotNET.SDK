// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dolittle.Events.Handling
{
    /// <summary>
    /// Defines an event handler that can handle events of type <typeparamref name="TEventType"/>.
    /// </summary>
    /// <typeparam name="TEventType">The type of events to handle.</typeparam>
    public interface IEventHandler<TEventType>
        where TEventType : IEvent
    {
        /// <summary>
        /// Gets the types of events that the handler handles.
        /// </summary>
        IEnumerable<Type> HandledEventTypes { get; }

        /// <summary>
        /// The method that is invoked to handle an <typeparamref name="TEventType"/>.
        /// </summary>
        /// <param name="event">The event to handle.</param>
        /// <param name="context">The <see cref="EventContext"/> for the event to handle.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task Handle(TEventType @event, EventContext context);
    }
}

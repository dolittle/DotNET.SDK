// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace Dolittle.Events.Handling.Builder
{
    /// <summary>
    /// Represents method to call on a <typeparamref name="THandlerType"/> when an event of type <typeparamref name="TEventType"/> is received.
    /// </summary>
    /// <param name="handler">The handler.</param>
    /// <param name="event">The event to handle.</param>
    /// <param name="context">The <see cref="EventContext"/> of the event to handle.</param>
    /// <typeparam name="THandlerType">The type of handler.</typeparam>
    /// <typeparam name="TEventType">The type of the event to handle.</typeparam>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public delegate Task EventHandlerMethod<THandlerType, TEventType>(THandlerType handler, TEventType @event, EventContext context)
        where TEventType : IEvent;
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Events.Processing.Internal;

namespace Dolittle.Events.Handling.Internal
{
    /// <summary>
    /// Defines a class used to construct instances of <see cref="EventHandlerProcessor{TEventType}"/>.
    /// </summary>
    public interface IEventHandlerProcessors
    {
        /// <summary>
        /// Creates an <see cref="IEventProcessor"/>.
        /// </summary>
        /// <param name="id">The unique <see cref="EventHandlerId"/> for the event handler.</param>
        /// <param name="scope">The <see cref="ScopeId"/> of the scope in the Event Store where the event handler will run.</param>
        /// <param name="partitioned">Whether the event handler should create a partitioned stream or not.</param>
        /// <param name="handler">The <see cref="IEventHandler{TEventType}"/> that will be called to handle incoming events.</param>
        /// <typeparam name="TEventType">The type of events to process.</typeparam>
        /// <returns>An <see cref="IEventProcessor"/>.</returns>
        IEventProcessor GetFor<TEventType>(
            EventHandlerId id,
            ScopeId scope,
            bool partitioned,
            IEventHandler<TEventType> handler)
            where TEventType : IEvent;
    }
}

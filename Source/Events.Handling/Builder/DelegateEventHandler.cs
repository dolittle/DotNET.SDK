// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dolittle.Events.Handling.Builder
{
    /// <summary>
    /// Defines an <see cref="IEventHandler{TEventType}"/> of type <typeparamref name="TEventType"/> that invokes the provided actions when events are received.
    /// </summary>
    /// <typeparam name="TEventType">The type of events to handle.</typeparam>
    public class DelegateEventHandler<TEventType> : IEventHandler<TEventType>
        where TEventType : IEvent
    {
        readonly IDictionary<Type, EventHandlerAction<TEventType>> _actions;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateEventHandler{TEventType}"/> class.
        /// </summary>
        public DelegateEventHandler()
        {
            _actions = new Dictionary<Type, EventHandlerAction<TEventType>>();
        }

        /// <inheritdoc/>
        public IEnumerable<Type> HandledEventTypes => _actions.Keys;

        /// <inheritdoc/>
        public Task Handle(TEventType @event, EventContext context)
        {
            var eventType = @event.GetType();
            if (_actions.TryGetValue(eventType, out var action))
            {
                return action.Invoke(@event, context);
            }

            throw new EventHandlerDoesNotHandleEvent(eventType);
        }

        /// <summary>
        /// Adds a <see cref="EventHandlerAction{TEventType}"/> of type <typeparamref name="TTEventType"/> to be performed when events of type <typeparamref name="TTEventType"/> are received.
        /// </summary>
        /// <typeparam name="TTEventType">The type of event to handle.</typeparam>
        /// <param name="action">The action to be performed when events of type <typeparamref name="TTEventType"/> are received.</param>
        public void AddAction<TTEventType>(EventHandlerAction<TEventType> action)
            where TTEventType : TEventType
            => _actions[typeof(TTEventType)] = action;
    }
}
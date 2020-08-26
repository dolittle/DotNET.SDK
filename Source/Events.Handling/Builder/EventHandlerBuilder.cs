// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.DependencyInversion;

namespace Dolittle.Events.Handling.Builder
{
    /// <summary>
    /// Defines a builder that can build instances of <see cref="IEventHandler{TEventType}"/> of type <typeparamref name="TEventType"/> from handler delegates.
    /// </summary>
    /// <typeparam name="TEventType">The type of events to handle.</typeparam>
    public class EventHandlerBuilder<TEventType>
        where TEventType : IEvent
    {
        readonly DelegateEventHandler<TEventType> _handler;

        internal EventHandlerBuilder()
        {
            _handler = new DelegateEventHandler<TEventType>();
        }

        /// <summary>
        /// Builds the <see cref="IEventHandler{TEventType}"/> of type <typeparamref name="TEventType"/> from the provided delegates.
        /// </summary>
        /// <returns>The composed <see cref="IEventHandler{TEventType}"/> of type <typeparamref name="TEventType"/>.</returns>
        public IEventHandler<TEventType> Build()
            => _handler;

        /// <summary>
        /// Adds a <see cref="EventHandlerAction{TEventType}"/> of type <typeparamref name="TTEventType"/> to be performed when events of type <typeparamref name="TTEventType"/> are received.
        /// </summary>
        /// <typeparam name="TTEventType">The type of event to handle.</typeparam>
        /// <param name="action">The action to be performed when events of type <typeparamref name="TTEventType"/> are received.</param>
        /// <returns>The <see cref="EventHandlerBuilder{TEventType}"/> for continued building.</returns>
        public EventHandlerBuilder<TEventType> Handle<TTEventType>(EventHandlerAction<TTEventType> action)
            where TTEventType : TEventType
        {
            _handler.AddAction<TTEventType>((@event, context) => action.Invoke((TTEventType)@event, context));
            return this;
        }

        /// <summary>
        /// Creates a builder to add methods to call on an instance of <typeparamref name="THandlerType"/>.
        /// </summary>
        /// <param name="instance">The instance of <typeparamref name="THandlerType"/> that will be used to call methods on when events are received.</param>
        /// <typeparam name="THandlerType">The type of the instance to call methods on.</typeparam>
        /// <returns>A <see cref="EventHandlerInstanceBuilder{THandlerType, TEventType}"/> to add methods to call on an instance of <typeparamref name="THandlerType"/>.</returns>
        public EventHandlerInstanceBuilder<THandlerType, TEventType> With<THandlerType>(THandlerType instance)
            => new EventHandlerInstanceBuilder<THandlerType, TEventType>(this, instance);

        /// <summary>
        /// Creates a builder to add methods to call on an instance of <typeparamref name="THandlerType"/> instantiate by a <see cref="FactoryFor{T}"/> of type <typeparamref name="THandlerType"/>.
        /// </summary>
        /// <param name="factory">The <see cref="FactoryFor{T}"/> of type <typeparamref name="THandlerType"/> use to instantiate the <typeparamref name="THandlerType"/>.</param>
        /// <typeparam name="THandlerType">The type of the instance to call methods on.</typeparam>
        /// <returns>A <see cref="EventHandlerFactoryBuilder{THandlerType, TEventType}"/> to add methods to call on an instance of <typeparamref name="THandlerType"/> instantiate by a <see cref="FactoryFor{T}"/> of type <typeparamref name="THandlerType"/>.</returns>
        public EventHandlerFactoryBuilder<THandlerType, TEventType> With<THandlerType>(FactoryFor<THandlerType> factory)
            => new EventHandlerFactoryBuilder<THandlerType, TEventType>(this, factory);
    }
}

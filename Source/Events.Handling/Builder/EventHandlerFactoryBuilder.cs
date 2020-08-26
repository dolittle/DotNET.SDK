// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.DependencyInversion;

namespace Dolittle.Events.Handling.Builder
{
    /// <summary>
    /// Defines a builder to add methods to call on an instance of <typeparamref name="THandlerType"/> instantiate by a <see cref="FactoryFor{T}"/> of type <typeparamref name="THandlerType"/>.
    /// </summary>
    /// <typeparam name="THandlerType">The type of instance to call methods on.</typeparam>
    /// <typeparam name="TEventType">The type of events to handle.</typeparam>
    public class EventHandlerFactoryBuilder<THandlerType, TEventType>
        where TEventType : IEvent
    {
        readonly EventHandlerBuilder<TEventType> _builder;
        readonly FactoryFor<THandlerType> _factory;

        internal EventHandlerFactoryBuilder(EventHandlerBuilder<TEventType> builder, FactoryFor<THandlerType> factory)
        {
            _builder = builder;
            _factory = factory;
        }

        /// <summary>
        /// Builds the <see cref="IEventHandler{TEventType}"/> of type <typeparamref name="TEventType"/> from the provided delegates.
        /// </summary>
        /// <returns>The composed <see cref="IEventHandler{TEventType}"/> of type <typeparamref name="TEventType"/>.</returns>
        public IEventHandler<TEventType> Build()
            => _builder.Build();

        /// <summary>
        /// Adds a <see cref="EventHandlerMethod{THandlerType, TTEventType}"/> to be called when events of type <typeparamref name="TTEventType"/> are received.
        /// </summary>
        /// <typeparam name="TTEventType">The type of event to handle.</typeparam>
        /// <param name="method">The method to call when events of type <typeparamref name="TTEventType"/> are received.</param>
        /// <returns>The <see cref="EventHandlerInstanceBuilder{THandlerType, TEventType}"/> for continued building.</returns>
        public EventHandlerFactoryBuilder<THandlerType, TEventType> Handle<TTEventType>(EventHandlerMethod<THandlerType, TTEventType> method)
            where TTEventType : TEventType
        {
            _builder.Handle<TTEventType>((@event, context) => method(_factory(), @event, context));
            return this;
        }
    }
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Types;

namespace Dolittle.Events.Handling.Internal
{
    /// <summary>
    /// Represents a system capable of providing implementations of event handlers.
    /// </summary>
    /// <typeparam name="THandlerType">The type of event handler to provide.</typeparam>
    /// <typeparam name="TEventType">The event type that the handler can handle.</typeparam>
    public interface ICanProvideHandlers<THandlerType, TEventType>
        where THandlerType : class, ICanHandle<TEventType>
        where TEventType : IEvent
    {
        /// <summary>
        /// Provides implementations of <typeparamref name="THandlerType"/>.
        /// </summary>
        /// <returns><see cref="IImplementationsOf{T}"/> of type <typeparamref name="THandlerType"/>.</returns>
        IImplementationsOf<THandlerType> Provide();
    }
}
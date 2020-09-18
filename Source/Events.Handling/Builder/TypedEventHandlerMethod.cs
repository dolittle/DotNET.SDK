// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Dolittle.SDK.Events.Handling.Builder
{
    /// <summary>
    /// An implementation of <see cref="IEventHandlerMethod" /> for an event handler method on an event of a specific type.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type" /> of the event.</typeparam>
    public class TypedEventHandlerMethod<T> : IEventHandlerMethod
        where T : class
    {
        readonly EventHandlerId _eventHandlerId;
        readonly TypedEventHandlerSignature<T> _method;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedEventHandlerMethod{T}"/> class.
        /// </summary>
        /// <param name="eventHandlerId">The <see cref="EventHandlerId" />.</param>
        /// <param name="method">The <see cref="EventHandlerSignature" />.</param>
        public TypedEventHandlerMethod(EventHandlerId eventHandlerId, TypedEventHandlerSignature<T> method)
        {
            _eventHandlerId = eventHandlerId;
            _method = method;
        }

        /// <inheritdoc/>
        Task Invoke(object @event, EventContext context)
        {
            if (@event is T typedEvent) return _method(typedEvent, context);
            throw new TypedEventHandlerMethodInvokedOnEventOfWrongType(_eventHandlerId, typeof(T), @event.GetType());
        }
    }
}

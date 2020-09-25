// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.SDK.Async;

namespace Dolittle.SDK.Events.Handling.Builder
{
    /// <summary>
    /// An implementation of <see cref="IEventHandlerMethod" /> for an event handler method on an event of a specific type.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type" /> of the event.</typeparam>
    public class TypedEventHandlerMethod<T> : IEventHandlerMethod
        where T : class
    {
        readonly Func<T, EventContext, Task> _method;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedEventHandlerMethod{T}"/> class.
        /// </summary>
        /// <param name="method">The <see cref="TypedEventHandlerSignature{T}" />.</param>
        public TypedEventHandlerMethod(TypedEventHandlerSignature<T> method)
            => _method = (T @event, EventContext context) => method(@event, context);

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedEventHandlerMethod{T}"/> class.
        /// </summary>
        /// <param name="method">The <see cref="VoidTypedEventHandlerSignature{T}" />.</param>
        public TypedEventHandlerMethod(VoidTypedEventHandlerSignature<T> method)
            => _method = (T @event, EventContext context) =>
            {
                method(@event, context);
                return Task.CompletedTask;
            };

        /// <inheritdoc/>
        public Task<Try> TryHandle(object @event, EventContext context)
        {
            if (@event is T typedEvent) return _method(typedEvent, context).TryTask();

            return Task.FromResult<Try>(new TypedEventHandlerMethodInvokedOnEventOfWrongType(typeof(T), @event.GetType()));
        }
    }
}

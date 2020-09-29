// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.SDK.Async;

namespace Dolittle.SDK.Events.Handling.Builder
{
    /// <summary>
    /// An implementation of <see cref="IEventHandlerMethod" /> that invokes a method on an event handler instance for an event of a specific type.
    /// </summary>
    /// <typeparam name="TEvent">The <see cref="Type" /> of the event.</typeparam>
    public class TypedInstanceEventHandlerMethod<TEvent> : IEventHandlerMethod
        where TEvent : class
    {
        readonly object _instance;
        readonly TaskEventHandlerMethodSignature<TEvent> _method;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedInstanceEventHandlerMethod{TEvent}"/> class.
        /// </summary>
        /// <param name="instance">The instance of the event handler.</param>
        /// <param name="method">The <see cref="TaskEventHandlerMethodSignature{TEvent}"/> method to invoke.</param>
        public TypedInstanceEventHandlerMethod(object instance, TaskEventHandlerMethodSignature<TEvent> method)
        {
            _instance = instance;
            _method = method;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedInstanceEventHandlerMethod{TEvent}"/> class.
        /// </summary>
        /// <param name="instance">The instance of the event handler.</param>
        /// <param name="method">The <see cref="VoidEventHandlerMethodSignature{TEvent}" /> method to invoke.</param>
        public TypedInstanceEventHandlerMethod(object instance, VoidEventHandlerMethodSignature<TEvent> method)
            : this(
                instance,
                (object instance, TEvent @event, EventContext context) =>
                {
                    method(instance, @event, context);
                    return Task.CompletedTask;
                })
        {
        }

        /// <inheritdoc/>
        public Task<Try> TryHandle(object @event, EventContext context)
        {
            if (@event is TEvent typedEvent) return _method(_instance, typedEvent, context).TryTask();

            return Task.FromResult<Try>(new TypedEventHandlerMethodInvokedOnEventOfWrongType(typeof(TEvent), @event.GetType()));
        }
    }
}

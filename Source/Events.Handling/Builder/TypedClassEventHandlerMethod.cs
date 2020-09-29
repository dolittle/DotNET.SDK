// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.SDK.Async;
using Dolittle.SDK.DependencyInversion;

namespace Dolittle.SDK.Events.Handling.Builder
{
    /// <summary>
    /// An implementation of <see cref="IEventHandlerMethod" /> that invokes a method on an event handler instance for an event of a specific type.
    /// </summary>
    /// <typeparam name="TEvent">The <see cref="Type" /> of the event.</typeparam>
    public class TypedClassEventHandlerMethod<TEvent> : IEventHandlerMethod
        where TEvent : class
    {
        readonly Type _eventHandlerType;
        readonly IContainer _container;
        readonly TaskEventHandlerMethodSignature<TEvent> _method;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedClassEventHandlerMethod{TEvent}"/> class.
        /// </summary>
        /// <param name="eventHandlerType">The <see cref="Type" /> of the event handler.</param>
        /// <param name="container">The <see cref="IContainer"/> to use for creating instances of the event handler.</param>
        /// <param name="method">The <see cref="TaskEventHandlerMethodSignature{TEvent}"/> method to invoke.</param>
        public TypedClassEventHandlerMethod(Type eventHandlerType, IContainer container, TaskEventHandlerMethodSignature<TEvent> method)
        {
            _eventHandlerType = eventHandlerType;
            _container = container;
            _method = method;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedClassEventHandlerMethod{TEvent}"/> class.
        /// </summary>
        /// <param name="eventHandlerType">The <see cref="Type" /> of the event handler.</param>
        /// <param name="container">The <see cref="IContainer"/> to use for creating instances of the event handler.</param>
        /// <param name="method">The <see cref="VoidEventHandlerMethodSignature{TEvent}" /> method to invoke.</param>
        public TypedClassEventHandlerMethod(Type eventHandlerType, IContainer container, VoidEventHandlerMethodSignature<TEvent> method)
            : this(
                eventHandlerType,
                container,
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
            if (@event is TEvent typedEvent) return _method(_container.Get(_eventHandlerType), typedEvent, context).TryTask();

            return Task.FromResult<Try>(new TypedEventHandlerMethodInvokedOnEventOfWrongType(typeof(TEvent), @event.GetType()));
        }
    }
}

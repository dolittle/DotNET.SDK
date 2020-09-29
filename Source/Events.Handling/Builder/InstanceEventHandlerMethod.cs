// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.SDK.Async;

namespace Dolittle.SDK.Events.Handling.Builder
{
    /// <summary>
    /// An implementation of <see cref="IEventHandlerMethod" /> that invokes a method on an event handler instance for an event of a specific type.
    /// </summary>
    public class InstanceEventHandlerMethod : IEventHandlerMethod
    {
        readonly object _instance;
        readonly TaskEventHandlerMethodSignature _method;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceEventHandlerMethod"/> class.
        /// </summary>
        /// <param name="instance">The instance of the event handler.</param>
        /// <param name="method">The <see cref="TaskEventHandlerMethodSignature{TEvent}"/> method to invoke.</param>
        public InstanceEventHandlerMethod(object instance, TaskEventHandlerMethodSignature method)
        {
            _instance = instance;
            _method = method;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceEventHandlerMethod"/> class.
        /// </summary>
        /// <param name="instance">The instance of the event handler.</param>
        /// <param name="method">The <see cref="VoidEventHandlerMethodSignature{TEvent}" /> method to invoke.</param>
        public InstanceEventHandlerMethod(object instance, VoidEventHandlerMethodSignature method)
            : this(
                instance,
                (object instance, object @event, EventContext context) =>
                {
                    method(instance, @event, context);
                    return Task.CompletedTask;
                })
        {
        }

        /// <inheritdoc/>
        public Task<Try> TryHandle(object @event, EventContext context)
            => _method(_instance, @event, context).TryTask();
    }
}

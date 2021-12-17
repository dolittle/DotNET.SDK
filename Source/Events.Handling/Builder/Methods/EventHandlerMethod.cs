// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.SDK.Async;

namespace Dolittle.SDK.Events.Handling.Builder.Methods
{
    /// <summary>
    /// An implementation of <see cref="IEventHandlerMethod" />.
    /// </summary>
    public class EventHandlerMethod : IEventHandlerMethod
    {
        readonly TaskEventHandlerSignature _method;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlerMethod"/> class.
        /// </summary>
        /// <param name="method">The <see cref="TaskEventHandlerSignature" />.</param>
        public EventHandlerMethod(TaskEventHandlerSignature method)
            => _method = method;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlerMethod"/> class.
        /// </summary>
        /// <param name="method">The <see cref="VoidEventHandlerSignature" />.</param>
        public EventHandlerMethod(VoidEventHandlerSignature method)
            => _method = (@event, context) =>
                {
                    method(@event, context);
                    return Task.CompletedTask;
                };

        /// <inheritdoc/>
        public Task<Try> TryHandle(object @event, EventContext context)
            => _method(@event, context).TryTask();
    }
}

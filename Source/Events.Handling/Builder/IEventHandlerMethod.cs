// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace Dolittle.SDK.Events.Handling.Builder
{
    /// <summary>
    /// Defines an event handler method.
    /// </summary>
    public interface IEventHandlerMethod
    {
        /// <summary>
        /// Invokes the event handler method.
        /// </summary>
        /// <param name="event">The event.</param>
        /// <param name="context">The <see cref="EventContext" />.</param>
        /// <returns>A <see cref="Task" /> representing the asynchronous action.</returns>
        Task Invoke(object @event, EventContext context);
    }
}

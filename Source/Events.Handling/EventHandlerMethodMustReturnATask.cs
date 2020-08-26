// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;

namespace Dolittle.Events.Handling
{
    /// <summary>
    /// Exception that gets thrown when an <see cref="MethodInfo"/> on an event handler is not asynchronous.
    /// </summary>
    public class EventHandlerMethodMustReturnATask : InvalidEventHandlerMethodSignature
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlerMethodMustReturnATask"/> class.
        /// </summary>
        /// <param name="methodInfo"><see cref="MethodInfo"/> that is violating.</param>
        public EventHandlerMethodMustReturnATask(MethodInfo methodInfo)
            : base(methodInfo, "The method must return a task.")
        {
        }
    }
}
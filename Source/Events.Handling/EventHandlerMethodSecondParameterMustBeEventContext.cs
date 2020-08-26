// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;

namespace Dolittle.Events.Handling
{
    /// <summary>
    /// Exception that gets thrown when an Event Handler handle method signature's second parameter is not <see cref="EventContext" />.
    /// </summary>
    public class EventHandlerMethodSecondParameterMustBeEventContext : InvalidEventHandlerMethodSignature
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlerMethodSecondParameterMustBeEventContext"/> class.
        /// </summary>
        /// <param name="methodInfo">The Event Handler Handle <see cref="MethodInfo" />.</param>
        public EventHandlerMethodSecondParameterMustBeEventContext(MethodInfo methodInfo)
            : base(methodInfo, $"The second parameter must be a {typeof(EventContext)}.")
        {
        }
    }
}
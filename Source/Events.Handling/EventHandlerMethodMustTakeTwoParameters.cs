// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;

namespace Dolittle.Events.Handling
{
    /// <summary>
    /// Exception that gets thrown when an Event Handler handle method signature does not take two paramters.
    /// </summary>
    public class EventHandlerMethodMustTakeTwoParameters : InvalidEventHandlerMethodSignature
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlerMethodMustTakeTwoParameters"/> class.
        /// </summary>
        /// <param name="methodInfo">The Event Handler Handle <see cref="MethodInfo" />.</param>
        public EventHandlerMethodMustTakeTwoParameters(MethodInfo methodInfo)
            : base(methodInfo, "The method must take exactly two parameter.")
        {
        }
    }
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;

namespace Dolittle.Events.Handling
{
    /// <summary>
    /// Exception that gets thrown when an Event Handler handle method signature's first parameter is not the correct event type.
    /// </summary>
    public class EventHandlerMethodFirstParameterMustBeCorrectEventType : InvalidEventHandlerMethodSignature
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlerMethodFirstParameterMustBeCorrectEventType"/> class.
        /// </summary>
        /// <param name="eventType">The expected event type.</param>
        /// <param name="methodInfo">The Event Handler Handle <see cref="MethodInfo" />.</param>
        public EventHandlerMethodFirstParameterMustBeCorrectEventType(Type eventType, MethodInfo methodInfo)
            : base(methodInfo, $"The first parameter must implement {eventType}.")
        {
        }
    }
}
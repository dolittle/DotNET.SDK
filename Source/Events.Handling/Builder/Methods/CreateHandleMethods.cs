// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;

namespace Dolittle.SDK.Events.Handling.Builder.Methods
{
    /// <summary>
    /// Create an untyped event handler method.
    /// </summary>
    /// <param name="method">The <see cref="MethodInfo" />.</param>
    /// <returns>The <see cref="IEventHandlerMethod" />.</returns>
    public delegate IEventHandlerMethod CreateUntypedHandleMethod(MethodInfo method);

    /// <summary>
    /// Create a typed event handler method.
    /// </summary>
    /// <param name="eventParameterType">The <see cref="Type" /> of the event to handle.</param>
    /// <param name="method">The <see cref="MethodInfo" /> of the handle method.</param>
    /// <returns>The <see cref="IEventHandlerMethod" />.</returns>
    public delegate IEventHandlerMethod CreateTypedHandleMethod(Type eventParameterType, MethodInfo method);
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Events.Handling.Builder.Methods
{
    /// <summary>
    /// Exception that gets thrown when a <see cref="TypedEventHandlerMethod{T}" /> is invoked on an event of the wrong type.
    /// </summary>
    public class TypedEventHandlerMethodInvokedOnEventOfWrongType : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypedEventHandlerMethodInvokedOnEventOfWrongType"/> class.
        /// </summary>
        /// <param name="expectedType">The expected <see cref="Type" />.</param>
        /// <param name="wrongType">The wrong <see cref="Type" />.</param>
        public TypedEventHandlerMethodInvokedOnEventOfWrongType(Type expectedType, Type wrongType)
            : base($"Could not handle event of type {wrongType} because it was expected to be of type {expectedType}")
        {
        }
    }
}
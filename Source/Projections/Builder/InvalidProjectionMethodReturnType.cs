// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Projections.Builder
{
    /// <summary>
    /// Exception that gets thrown when a projection method has an invalid return type.
    /// </summary>
    public class InvalidProjectionMethodReturnType : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidProjectionMethodReturnType"/> class.
        /// </summary>
        /// <param name="returnType">The return <see cref="Type" /> of the projection method.</param>
        public InvalidProjectionMethodReturnType(Type returnType)
            : base($"{returnType} is not a valid projection method")
        {
        }
    }
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Embeddings.Builder
{
    /// <summary>
    /// Exception that gets thrown when an update method has an invalid return type.
    /// </summary>
    public class InvalidUpdateMethodReturnType : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidUpdateMethodReturnType"/> class.
        /// </summary>
        /// <param name="returnType">The return <see cref="Type" /> of the update method.</param>
        public InvalidUpdateMethodReturnType(Type returnType)
            : base($"{returnType} is not a valid update method return type.")
        {
        }
    }
}

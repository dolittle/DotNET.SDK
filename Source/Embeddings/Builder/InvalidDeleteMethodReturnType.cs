// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Embeddings.Builder
{
    /// <summary>
    /// Exception that gets thrown when a deletion method has an invalid return type.
    /// </summary>
    public class InvalidDeleteMethodReturnType : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidDeleteMethodReturnType"/> class.
        /// </summary>
        /// <param name="returnType">The return <see cref="Type" /> of the deletion method.</param>
        public InvalidDeleteMethodReturnType(Type returnType)
            : base($"{returnType} is not a valid deletion method return type.")
        {
        }
    }
}

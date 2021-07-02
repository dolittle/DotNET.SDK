// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Embeddings.Builder
{
    /// <summary>
    /// Exception that gets thrown when a compare method has an invalid return type.
    /// </summary>
    public class InvalidCompareMethodReturnType : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCompareMethodReturnType"/> class.
        /// </summary>
        /// <param name="returnType">The return <see cref="Type" /> of the compare method.</param>
        public InvalidCompareMethodReturnType(Type returnType)
            : base($"{returnType} is not a valid compare method return type.")
        {
        }
    }
}

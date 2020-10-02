// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Protobuf.Contracts;

namespace Dolittle.SDK.Protobuf
{
    /// <summary>
    /// Exception that gets thrown when an error occurs converting between <see cref="Guid"/> and <see cref="Uuid"/>.
    /// </summary>
    public class InvalidGuidConversion : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidGuidConversion"/> class.
        /// </summary>
        /// <param name="details">The details on why conversion failed.</param>
        public InvalidGuidConversion(string details)
            : base($"Could not convert to guid because {details}")
        {
        }
    }
}
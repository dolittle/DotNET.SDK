// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Protobuf
{
    /// <summary>
    /// Exception that gets thrown when there is information missing on an execution context.
    /// </summary>
    public class MissingExecutionContextInformation : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingExecutionContextInformation"/> class.
        /// </summary>
        /// <param name="details">The details on what is missing.</param>
        public MissingExecutionContextInformation(string details)
            : base($"Execution context is missing information about its {details}")
        {
        }
    }
}

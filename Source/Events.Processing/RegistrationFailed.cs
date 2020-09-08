// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Protobuf.Contracts;

namespace Dolittle.SDK.Events.Processing
{
    /// <summary>
    /// Exception that gets thrown when a failure occurs during registration of an event processor.
    /// </summary>
    public class RegistrationFailed : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationFailed"/> class.
        /// </summary>
        /// <param name="kind">The kind of the event processor.</param>
        /// <param name="identifier">The identifier of the event processor.</param>
        /// <param name="failure">The <see cref="Failure" />.</param>
        public RegistrationFailed(string kind, Guid identifier, Failure failure)
            : base($"Failure occurred during registration of ${kind} ${identifier}. ${failure.Reason}")
        {
        }
    }
}
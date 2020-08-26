// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Concepts;
using Dolittle.Protobuf;

namespace Dolittle.Events.Processing
{
    /// <summary>
    /// Exception that gets thrown when a failure occurs during registration of an event handler.
    /// </summary>
    public class RegistrationFailed : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationFailed"/> class.
        /// </summary>
        /// <param name="kind">The kind of the event processor.</param>
        /// <param name="id">The <see cref="ConceptAs{T}"/> of type <see cref="Guid"/> that identifies the event processor.</param>
        /// <param name="registerFailure">The <see cref="Failure"/> that occured.</param>
        public RegistrationFailed(string kind, ConceptAs<Guid> id, Failure registerFailure)
            : base($"Failure occurred during registration of {kind} {id}. {registerFailure.Reason}")
        {
        }
    }
}
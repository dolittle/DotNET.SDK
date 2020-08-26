// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Concepts;

namespace Dolittle.Events.Processing
{
    /// <summary>
    /// Exception that gets thrown when registration response is recieved after registering an event processor.
    /// </summary>
    public class DidNotReceiveRegistrationResponse : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DidNotReceiveRegistrationResponse"/> class.
        /// </summary>
        /// <param name="kind">The kind of the event processor.</param>
        /// <param name="id">The <see cref="ConceptAs{T}"/> of type <see cref="Guid"/> that identifies the event processor.</param>
        public DidNotReceiveRegistrationResponse(string kind, ConceptAs<Guid> id)
            : base($"Did not receive registration response while registering {kind} {id}")
        {
        }
    }
}
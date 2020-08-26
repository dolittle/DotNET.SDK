// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Events.Filters
{
    /// <summary>
    /// Exception that gets thrown when no filter registration response is recieved after registering an event filter.
    /// </summary>
    public class DidNotReceiveFilterRegistrationResponse : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DidNotReceiveFilterRegistrationResponse"/> class.
        /// </summary>
        /// <param name="id">The unique <see cref="FilterId"/> for the filter.</param>
        public DidNotReceiveFilterRegistrationResponse(FilterId id)
            : base($"Did not receive filter registration response while registering filter {id}")
        {
        }
    }
}
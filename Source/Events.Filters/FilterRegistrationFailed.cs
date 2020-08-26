// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Protobuf;

namespace Dolittle.Events.Filters
{
    /// <summary>
    /// Exception that gets thrown when a failure occurs during registration of an event filter.
    /// </summary>
    public class FilterRegistrationFailed : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterRegistrationFailed"/> class.
        /// </summary>
        /// <param name="id">The unique <see cref="FilterId"/> for the filter.</param>
        /// <param name="registerFailure">The <see cref="Failure"/> that occured.</param>
        public FilterRegistrationFailed(FilterId id, Failure registerFailure)
            : base($"Failure occured during registration of filter {id}. {registerFailure.Reason}")
        {
        }
    }
}
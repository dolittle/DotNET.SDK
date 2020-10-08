// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Failures.EventHorizon
{
    /// <summary>
    /// Exception that gets thrown when trying to subscribe to events from a microservice the runtime does not know about.
    /// </summary>
    public class MissingMicroserviceConfiguration : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingMicroserviceConfiguration"/> class.
        /// </summary>
        /// <param name="reason">The failure reason.</param>
        public MissingMicroserviceConfiguration(string reason)
            : base(reason)
        {
        }
    }
}
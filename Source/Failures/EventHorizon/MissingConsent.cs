// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Failures.EventHorizon
{
    /// <summary>
    /// Exception that gets thrown when trying to subscribe to events from a microservice that has not authorized the subscription.
    /// </summary>
    public class MissingConsent : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingConsent"/> class.
        /// </summary>
        /// <param name="reason">The failure reason.</param>
        public MissingConsent(string reason)
            : base(reason)
        {
        }
    }
}
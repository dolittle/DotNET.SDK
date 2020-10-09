// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Failures.EventHorizon
{
    /// <summary>
    /// Exception that gets thrown when the runtime does not recieve a response from the producer runtime.
    /// </summary>
    public class DidNotReceiveSubscriptionResponse : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DidNotReceiveSubscriptionResponse"/> class.
        /// </summary>
        /// <param name="reason">The failure reason.</param>
        public DidNotReceiveSubscriptionResponse(string reason)
            : base(reason)
        {
        }
    }
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.EventHorizon
{
    /// <summary>
    /// Exception that gets thrown when a method is called more than once while building an event horizon subscription.
    /// </summary>
    public class SubscriptionBuilderMethodAlreadyCalled : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionBuilderMethodAlreadyCalled"/> class.
        /// </summary>
        /// <param name="method">The method that was called more than once.</param>
        public SubscriptionBuilderMethodAlreadyCalled(string method)
            : base($"The method {method} can only be called once while building an Event Horizon Subscription.")
        {
        }
    }
}

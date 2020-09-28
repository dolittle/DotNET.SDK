// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Protobuf;

namespace Dolittle.SDK.EventHorizon
{
    /// <summary>
    /// Represents the response from the Runtime to an Event Horizon Subscribe request.
    /// </summary>
    public class SubscribeResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubscribeResponse"/> class.
        /// </summary>
        /// <param name="subscription">The <see cref="Subscription"/> that the response refers to.</param>
        /// <param name="consent">The <see cref="ConsentId"/> that authorized the subscription.</param>
        /// <param name="failure">The <see cref="Failure"/> describing why the subscription failed.</param>
        public SubscribeResponse(Subscription subscription, ConsentId consent, Failure failure)
        {
            Subscription = subscription;
            Consent = consent;
            Failure = failure;
        }

        /// <summary>
        /// Gets the <see cref="Subscription"/> that the response refers to.
        /// </summary>
        public Subscription Subscription { get; }

        /// <summary>
        /// Gets a value indicating whether or not the subscribe request failed.
        /// </summary>
        public bool Failed => Failure != null;

        /// <summary>
        /// Gets the <see cref="ConsentId"/> that authorized the subscription, if it succeeded.
        /// </summary>
        public ConsentId Consent { get; }

        /// <summary>
        /// Gets the <see cref="Failure"/> describing why the subscription failed, if it failed.
        /// </summary>
        public Failure Failure { get; }
    }
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Protobuf;
using Contracts = Dolittle.Protobuf.Contracts;

namespace Dolittle.EventHorizon
{
    /// <summary>
    /// Represents the result of an event horizon subsctiption request.
    /// </summary>
    public class SubscriptionResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionResponse"/> class.
        /// </summary>
        /// <param name="consentId">The optional <see cref="Contracts.Uuid"/> that represents the <see cref="ConsentId"/> for this subscription.</param>
        /// <param name="failure">The optional <see cref="Contracts.Failure"/> that occured during the subscription request.</param>
        public SubscriptionResponse(Contracts.Uuid consentId, Contracts.Failure failure)
        {
            if (consentId != null) Consent = consentId.To<ConsentId>();
            if (failure != null) Failure = failure;
        }

        /// <summary>
        /// Gets a value indicating whether the subscription was successful or not.
        /// </summary>
        public bool Success => Failure == null;

        /// <summary>
        /// Gets the <see cref="ConsentId"/> that was given to allow this subscription.
        /// </summary>
        public ConsentId Consent { get; }

        /// <summary>
        /// Gets the subscription response failure.
        /// </summary>
        public Failure Failure { get; }
    }
}
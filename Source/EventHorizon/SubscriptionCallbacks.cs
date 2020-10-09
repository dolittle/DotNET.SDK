// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Failures;

namespace Dolittle.SDK.EventHorizon
{
    /// <summary>
    /// Callback that gets called when an Event Horizon Subscription succeeds.
    /// </summary>
    /// <param name="subscription">The <see cref="Subscription"/> the callback refers to.</param>
    /// <param name="consent">The <see cref="ConsentId"/> that was used to authorize the subscription.</param>
    public delegate void SubscriptionSucceeded(Subscription subscription, ConsentId consent);

    /// <summary>
    /// Callback that gets called when an Event Horizon Subscription fails.
    /// </summary>
    /// <param name="subscription">The <see cref="Subscription"/> the callback refers to.</param>
    /// <param name="failure">The <see cref="Failure"/> that caused the subscription to fail.</param>
    public delegate void SubscriptionFailed(Subscription subscription, Failure failure);

    /// <summary>
    /// Callback that gets called when an Event Horizon Subscription completes.
    /// </summary>
    /// <param name="response">The <see cref="SubscribeResponse"/> returned from the Runtime.</param>
    public delegate void SubscriptionCompleted(SubscribeResponse response);

    /// <summary>
    /// Represents an observer that holds callbacks for subscription results.
    /// </summary>
    public class SubscriptionCallbacks : IObserver<SubscribeResponse>
    {
        /// <summary>
        /// Occurs when a subscription succeeds.
        /// </summary>
        public event SubscriptionSucceeded OnSuccess;

        /// <summary>
        /// Occurs when a subscription fails.
        /// </summary>
        public event SubscriptionFailed OnFailure;

        /// <summary>
        /// Occurs when a subscription completes.
        /// </summary>
        public event SubscriptionCompleted OnCompleted;

        /// <inheritdoc/>
        public void OnNext(SubscribeResponse value)
        {
            if (!value.Failed)
            {
                OnSuccess?.Invoke(value.Subscription, value.Consent);
            }
            else
            {
                OnFailure?.Invoke(value.Subscription, value.Failure);
            }

            OnCompleted?.Invoke(value);
        }

        /// <inheritdoc/>
        public void OnError(Exception error)
        {
        }

        /// <inheritdoc/>
        void IObserver<SubscribeResponse>.OnCompleted()
        {
        }
    }
}
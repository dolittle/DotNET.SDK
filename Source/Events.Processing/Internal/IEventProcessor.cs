// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;

namespace Dolittle.SDK.Events.Processing.Internal
{
    /// <summary>
    /// Defines a system that handles the behavior of event processors that registers with the Runtime and handles processing requests.
    /// </summary>
    /// <typeparam name="TRegisterResponse">The <see cref="Type" /> of the registration response.</typeparam>
    public interface IEventProcessor<TRegisterResponse>
        where TRegisterResponse : class
    {
        /// <summary>
        /// Registers the event processor with the Runtime, and if successful starts handling requests.
        /// </summary>
        /// <param name="cancellation">The <see cref="CancellationToken" /> used to cancel the registration and processing.</param>
        /// <returns>An <see cref="IObservable{T}" /> representing the connection to the Runtime.</returns>
        IObservable<TRegisterResponse> Register(CancellationToken cancellation);

        /// <summary>
        /// Register the event processor with the Runtime with a <see cref="RetryPolicy" />, and if successful starts handling requests.
        /// </summary>
        /// <param name="policy">The <see cref="RetryPolicy" /> to use when trying to reconnect.</param>
        /// <param name="cancellation">The <see cref="CancellationToken" /> used to cancel the registration and processing.</param>
        /// <returns>An <see cref="IObservable{T}" /> representing the connection to the runtime.</returns>
        IObservable<TRegisterResponse> RegisterWithPolicy(RetryPolicy policy, CancellationToken cancellation);

        /// <summary>
        /// Register the event processor with the Runtime with a <see cref="RetryPolicy" /> that will try forever, and if successful starts handling requests.
        /// </summary>
        /// <param name="policy">The <see cref="RetryPolicy" /> to use when trying to reconnect.</param>
        /// <param name="cancellation">The <see cref="CancellationToken" /> used to cancel the registration and processing.</param>
        /// <returns>An <see cref="IObservable{T}" /> representing the connection to the runtime.</returns>
        IObservable<TRegisterResponse> RegisterForeverWithPolicy(RetryPolicy policy, CancellationToken cancellation);
    }

#pragma warning disable SA1649
    /// <summary>
    /// Something.
    /// </summary>
    public class RetryPolicy
    {
    }
#pragma warning restore
}

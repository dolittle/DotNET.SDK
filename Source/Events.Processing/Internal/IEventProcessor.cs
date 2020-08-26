// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Resilience;

namespace Dolittle.Events.Processing.Internal
{
    /// <summary>
    /// Defines a system that handles the behaviour of event processors that registers with the Runtime and handles processing requests.
    /// </summary>
    public interface IEventProcessor
    {
        /// <summary>
        /// Registers the event processor with the Runtime, and if successful starts handling requests.
        /// </summary>
        /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task RegisterAndHandle(CancellationToken cancellationToken);

        /// <summary>
        /// Registers the event processor with the Runtime, and if successful starts handling requests.
        /// If the processor fails during registration or handling, the provided policy will be used to retry.
        /// If the processor finishes handling of requests without failing, it will restart.
        /// </summary>
        /// <param name="policy">The <see cref="IAsyncPolicy"/> that defines the retry behaviour upon failure.</param>
        /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task RegisterAndHandleForeverWithPolicy(IAsyncPolicy policy, CancellationToken cancellationToken);

        /// <summary>
        /// Registers the event processor with the Runtime, and if successful starts handling requests.
        /// If the processor fails during registration or handling, the provided policy will be used to retry.
        /// </summary>
        /// <param name="policy">The <see cref="IAsyncPolicy"/> that defines the retry behaviour upon failure.</param>
        /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task RegisterAndHandleWithPolicy(IAsyncPolicy policy, CancellationToken cancellationToken);
    }
}

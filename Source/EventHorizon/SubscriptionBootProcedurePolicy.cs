// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Logging;
using Dolittle.Resilience;
using Grpc.Core;
using Polly;

namespace Dolittle.EventHorizon
{
    /// <summary>
    /// Defines the policy for registering event horizons subscriptions to the Runtime.
    /// </summary>
    public class SubscriptionBootProcedurePolicy : IDefineAsyncPolicyForType
    {
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionBootProcedurePolicy"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to use for logging.</param>
        public SubscriptionBootProcedurePolicy(ILogger<SubscriptionBootProcedure> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public Type Type => typeof(SubscriptionBootProcedure);

        /// <inheritdoc/>
        public Polly.IAsyncPolicy Define()
            => Polly.Policy
                .Handle<Exception>(
                    _ =>
                    {
                        if (_ is RpcException rpcException && rpcException.StatusCode == StatusCode.Unavailable)
                            return true;
                        if (_ is FailedToSubscribeToEventHorizon)
                        {
                            _logger.Warning(_.Message);
                            return true;
                        }

                        _logger.Warning(_, "Error while subscribing to event horizon");
                        return true;
                    })
                .WaitAndRetryForeverAsync(attempt => TimeSpan.FromSeconds(Math.Min(Math.Pow(2, attempt), 60)));
    }
}

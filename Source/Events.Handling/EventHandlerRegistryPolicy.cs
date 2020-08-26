// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Logging;
using Dolittle.Resilience;
using Grpc.Core;
using Polly;

namespace Dolittle.Events.Handling
{
    /// <summary>
    /// Defines the policy for registering event handlers to the Runtime.
    /// </summary>
    public class EventHandlerRegistryPolicy : IDefineAsyncPolicyForType
    {
        readonly ILogger<EventHandlerRegistry> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlerRegistryPolicy"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to use for logging.</param>
        public EventHandlerRegistryPolicy(ILogger<EventHandlerRegistry> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public Type Type => typeof(EventHandlerRegistry);

        /// <inheritdoc/>
        public Polly.IAsyncPolicy Define()
            => Polly.Policy
                .Handle<Exception>(
                    _ =>
                    {
                        if (_ is RpcException rpcException && rpcException.StatusCode == StatusCode.Unavailable)
                            return true;

                        _logger.Warning(_, "Error while registering event handler");
                        return true;
                    })
                .WaitAndRetryForeverAsync(attempt => TimeSpan.FromSeconds(Math.Min(Math.Pow(2, attempt), 60)));
    }
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Contracts;
using Dolittle.SDK.Services;
using Grpc.Core;

namespace Dolittle.SDK.Events.Internal
{
    /// <summary>
    /// Represents a wrapper for gRPC Subscriptions.Subscribe.
    /// </summary>
    public class EventTypesRegisterMethod : ICanCallAUnaryMethod<EventTypeRegistrationRequest, EventTypeRegistrationResponse>
    {
        /// <inheritdoc/>
        public AsyncUnaryCall<EventTypeRegistrationResponse> Call(EventTypeRegistrationRequest message, Channel channel, CallOptions callOptions)
        {
            var client = new Dolittle.Runtime.Events.Contracts.EventTypes.EventTypesClient(channel);
            return client.RegisterAsync(message, callOptions);
        }
    }
}
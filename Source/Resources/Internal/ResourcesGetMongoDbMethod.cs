// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Resources.Contracts;
using Dolittle.SDK.Services;
using Grpc.Core;

namespace Dolittle.SDK.Resources.Internal
{
    /// <summary>
    /// Represents a wrapper for gRPC Subscriptions.Subscribe.
    /// </summary>
    public class ResourcesGetMongoDbMethod : ICanCallAUnaryMethod<GetRequest, GetMongoDbResponse>
    {
        /// <inheritdoc/>
        public AsyncUnaryCall<GetMongoDbResponse> Call(GetRequest message, Channel channel, CallOptions callOptions)
        {
            var client = new Dolittle.Runtime.Resources.Contracts.Resources.ResourcesClient(channel);
            return client.GetMongoDbAsync(message, callOptions);
        }
    }
}
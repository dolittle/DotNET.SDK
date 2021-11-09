// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Tenancy.Contracts;
using Dolittle.SDK.Services;
using Grpc.Core;

namespace Dolittle.SDK.Tenancy.Internal
{
    /// <summary>
    /// Represents a wrapper for gRPC Subscriptions.Subscribe.
    /// </summary>
    public class TenantsGetAllMethod : ICanCallAUnaryMethod<GetAllRequest, GetAllResponse>
    {
        /// <inheritdoc/>
        public AsyncUnaryCall<GetAllResponse> Call(GetAllRequest message, Channel channel, CallOptions callOptions)
        {
            var client = new Dolittle.Runtime.Tenancy.Contracts.Tenants.TenantsClient(channel);
            return client.GetAllAsync(message, callOptions);
        }
    }
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Projections.Contracts;
using Dolittle.SDK.Services;
using Grpc.Core;
using static Dolittle.Runtime.Projections.Contracts.Projections;

namespace Dolittle.SDK.Projections.Store
{
    /// <summary>
    /// Represents a wrapper for gRPC EventStore.Commit.
    /// </summary>
    public class ProjectionsGetAll : ICanCallAUnaryMethod<GetAllRequest, GetAllResponse>
    {
        /// <inheritdoc/>
        public AsyncUnaryCall<GetAllResponse> Call(GetAllRequest message, Channel channel, CallOptions callOptions)
        {
            var client = new ProjectionsClient(channel);
            return client.GetAllAsync(message, callOptions);
        }
    }
}

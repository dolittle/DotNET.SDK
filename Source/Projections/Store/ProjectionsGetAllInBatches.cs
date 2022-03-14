// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Projections.Contracts;
using Dolittle.SDK.Services;
using Grpc.Core;

namespace Dolittle.SDK.Projections.Store;

/// <summary>
/// Represents a wrapper for gRPC Projections.GetAllInBatches
/// </summary>
public class ProjectionsGetAllInBatches : ICanCallAServerStreamingMethod<GetAllRequest, GetAllResponse>
{
    /// <inheritdoc/>
    public AsyncServerStreamingCall<GetAllResponse> Call(GetAllRequest message, ChannelBase channel, CallOptions callOptions)
    {
        var client = new Runtime.Projections.Contracts.Projections.ProjectionsClient(channel);
        return client.GetAllInBatches(message, callOptions);
    }
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Resources.Contracts;
using Dolittle.SDK.Services;
using Grpc.Core;

namespace Dolittle.SDK.Resources.MongoDB.Internal;

/// <summary>
/// Represents a wrapper for gRPC Resources.GetMongoDB.
/// </summary>
public class ResourcesGetMongoDBMethod : ICanCallAUnaryMethod<GetRequest, GetMongoDBResponse>
{
    /// <inheritdoc/>
    public AsyncUnaryCall<GetMongoDBResponse> Call(GetRequest message, ChannelBase channel, CallOptions callOptions)
    {
        var client = new Dolittle.Runtime.Resources.Contracts.Resources.ResourcesClient(channel);
        return client.GetMongoDBAsync(message, callOptions);
    }
}

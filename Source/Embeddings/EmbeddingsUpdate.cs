// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Embeddings.Contracts;
using Dolittle.SDK.Services;
using Grpc.Core;
using static Dolittle.Runtime.Embeddings.Contracts.Embeddings;

namespace Dolittle.SDK.Embeddings;

/// <summary>
/// Represents a wrapper for gRPC Embeddings.Update.
/// </summary>
public class EmbeddingsUpdate : ICanCallAUnaryMethod<UpdateRequest, UpdateResponse>
{
    /// <inheritdoc/>
    public AsyncUnaryCall<UpdateResponse> Call(UpdateRequest message, ChannelBase channel, CallOptions callOptions)
    {
        var client = new EmbeddingsClient(channel);
        return client.UpdateAsync(message, callOptions);
    }
}

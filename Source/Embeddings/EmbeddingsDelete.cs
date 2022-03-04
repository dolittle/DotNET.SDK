// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Embeddings.Contracts;
using Dolittle.SDK.Services;
using Grpc.Core;
using static Dolittle.Runtime.Embeddings.Contracts.Embeddings;

namespace Dolittle.SDK.Embeddings;

/// <summary>
/// Represents a wrapper for gRPC Embeddings.Delete.
/// </summary>
public class EmbeddingsDelete : ICanCallAUnaryMethod<DeleteRequest, DeleteResponse>
{
    /// <inheritdoc/>
    public AsyncUnaryCall<DeleteResponse> Call(DeleteRequest message, ChannelBase channel, CallOptions callOptions)
    {
        var client = new EmbeddingsClient(channel);
        return client.DeleteAsync(message, callOptions);
    }
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Embeddings.Contracts;
using Dolittle.SDK.Services;
using Grpc.Core;
using static Dolittle.Runtime.Embeddings.Contracts.EmbeddingStore;

namespace Dolittle.SDK.Embeddings.Store;

/// <summary>
/// Represents a wrapper for gRPC EmbeddingStore.GetAll.
/// </summary>
public class EmbeddingsGetAll : ICanCallAUnaryMethod<GetAllRequest, GetAllResponse>
{
    /// <inheritdoc/>
    public AsyncUnaryCall<GetAllResponse> Call(GetAllRequest message, Channel channel, CallOptions callOptions)
    {
        var client = new EmbeddingStoreClient(channel);
        return client.GetAllAsync(message, callOptions);
    }
}
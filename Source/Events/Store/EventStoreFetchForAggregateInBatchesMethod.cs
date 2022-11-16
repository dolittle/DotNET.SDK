// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Contracts;
using Dolittle.SDK.Services;
using Grpc.Core;

namespace Dolittle.SDK.Events.Store;

/// <summary>
/// Represents a wrapper for gRPC EventStore.FetchForAggregateInBatches.
/// </summary>
public class EventStoreFetchForAggregateInBatchesMethod : ICanCallAServerStreamingMethod<FetchForAggregateInBatchesRequest, FetchForAggregateResponse>
{
    /// <inheritdoc/>
    public AsyncServerStreamingCall<FetchForAggregateResponse> Call(FetchForAggregateInBatchesRequest message, ChannelBase channel, CallOptions callOptions)
    {
        var client = new Runtime.Events.Contracts.EventStore.EventStoreClient(channel);
        return client.FetchForAggregateInBatches(message, callOptions);
    }
}

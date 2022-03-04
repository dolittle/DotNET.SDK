// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Contracts;
using Dolittle.SDK.Services;
using Grpc.Core;
using static Dolittle.Runtime.Events.Contracts.EventStore;

namespace Dolittle.SDK.Events.Store;

/// <summary>
/// Represents a wrapper for gRPC EventStore.FetchForAggregate.
/// </summary>
public class EventStoreFetchForAggregateMethod : ICanCallAUnaryMethod<FetchForAggregateRequest, FetchForAggregateResponse>
{
    /// <inheritdoc/>
    public AsyncUnaryCall<FetchForAggregateResponse> Call(FetchForAggregateRequest message, ChannelBase channel, CallOptions callOptions)
    {
        var client = new EventStoreClient(channel);
        return client.FetchForAggregateAsync(message, callOptions);
    }
}

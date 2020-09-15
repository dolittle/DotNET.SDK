// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Contracts;
using Dolittle.SDK.Services;
using Grpc.Core;
using static Dolittle.Runtime.Events.Contracts.EventStore;
using Contracts = Dolittle.Runtime.Events.Contracts;

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Represents a wrapper for gRPC EventStore.Commit.
    /// </summary>
    public class EventStoreCommitMethod : ICanCallAnUnaryMethod<EventStoreClient, CommitEventsRequest, Contracts.CommitEventsResponse>
    {
        /// <inheritdoc/>
        public AsyncUnaryCall<Contracts.CommitEventsResponse> Call(CommitEventsRequest message, Channel channel, CallOptions callOptions)
        {
            var client = new EventStoreClient(channel);
            return client.CommitAsync(message, callOptions);
        }
    }
}

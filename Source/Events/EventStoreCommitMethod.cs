// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using static Dolittle.Runtime.Events.Contracts.EventStore;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.SDK.Services;
using Grpc.Core;

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Represents a wrapper for gRPC EventStore.Commit.
    /// </summary>
    public class EventStoreCommitMethod : ICanCallAnUnaryMethod<EventStoreClient, CommitEventsRequest, CommitEventsResponse>
    {

        /// <inheritdoc/>
        public AsyncUnaryCall<CommitEventsResponse> Call(CommitEventsRequest message, Channel channel, CallOptions callOptions)
        {
            var client = new EventStoreClient(channel);
            return client.CommitAsync(message, callOptions);
        }
    }
}

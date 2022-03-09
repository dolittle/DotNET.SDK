// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Aggregates.Contracts;
using Dolittle.SDK.Services;
using Grpc.Core;

namespace Dolittle.SDK.Aggregates.Internal;

/// <summary>
/// Represents a wrapper for gRPC AggregateRoots.RegisterAlias.
/// </summary>
public class AggregateRootsRegisterAliasMethod : ICanCallAUnaryMethod<AggregateRootAliasRegistrationRequest, AggregateRootAliasRegistrationResponse>
{
    /// <inheritdoc/>
    public AsyncUnaryCall<AggregateRootAliasRegistrationResponse> Call(AggregateRootAliasRegistrationRequest message, ChannelBase channel, CallOptions callOptions)
    {
        var client = new Dolittle.Runtime.Aggregates.Contracts.AggregateRoots.AggregateRootsClient(channel);
        return client.RegisterAliasAsync(message, callOptions);
    }
}

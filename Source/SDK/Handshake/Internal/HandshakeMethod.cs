// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Handshake.Contracts;
using Dolittle.SDK.Services;
using Grpc.Core;

namespace Dolittle.SDK.Handshake.Internal;

/// <summary>
/// Represents a wrapper for gRPC Handshake.Handshake.
/// </summary>
public class HandshakeMethod : ICanCallAUnaryMethod<HandshakeRequest, HandshakeResponse>
{
    /// <inheritdoc/>
    public AsyncUnaryCall<HandshakeResponse> Call(HandshakeRequest message, Channel channel, CallOptions callOptions)
    {
        var client = new Dolittle.Runtime.Handshake.Contracts.Handshake.HandshakeClient(channel);
        return client.HandshakeAsync(message, callOptions);
    }
}
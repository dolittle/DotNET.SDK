// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Protobuf.Contracts;
using Dolittle.Runtime.Embeddings.Contracts;
using Dolittle.SDK.Services;
using Dolittle.Services.Contracts;
using Grpc.Core;
using static Dolittle.Runtime.Embeddings.Contracts.Embeddings;

namespace Dolittle.SDK.Embeddings.Internal;

/// <summary>
/// An implementation of <see cref="IAmAReverseCallProtocol{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}" /> for embeddings.
/// </summary>
public class EmbeddingsProtocol : IAmAReverseCallProtocol<EmbeddingClientToRuntimeMessage, EmbeddingRuntimeToClientMessage, EmbeddingRegistrationRequest, EmbeddingRegistrationResponse, EmbeddingRequest, EmbeddingResponse>
{
    /// <inheritdoc/>
    public AsyncDuplexStreamingCall<EmbeddingClientToRuntimeMessage, EmbeddingRuntimeToClientMessage> Call(ChannelBase channel, CallOptions callOptions)
        => new EmbeddingsClient(channel).Connect(callOptions);

    /// <inheritdoc/>
    public EmbeddingClientToRuntimeMessage CreateMessageFrom(EmbeddingRegistrationRequest arguments)
        => new()
            { RegistrationRequest = arguments };

    /// <inheritdoc/>
    public EmbeddingClientToRuntimeMessage CreateMessageFrom(Pong pong)
        => new()
            { Pong = pong };

    /// <inheritdoc/>
    public EmbeddingClientToRuntimeMessage CreateMessageFrom(EmbeddingResponse response)
        => new()
            { HandleResult = response };

    /// <inheritdoc/>
    public EmbeddingRegistrationResponse GetConnectResponseFrom(EmbeddingRuntimeToClientMessage message)
        => message.RegistrationResponse;

    /// <inheritdoc/>
    public Failure GetFailureFromConnectResponse(EmbeddingRegistrationResponse response)
        => response.Failure;

    /// <inheritdoc/>
    public Ping GetPingFrom(EmbeddingRuntimeToClientMessage message)
        => message.Ping;

    /// <inheritdoc/>
    public ReverseCallRequestContext GetRequestContextFrom(EmbeddingRequest message)
        => message.CallContext;

    /// <inheritdoc/>
    public EmbeddingRequest GetRequestFrom(EmbeddingRuntimeToClientMessage message)
        => message.HandleRequest;

    /// <inheritdoc/>
    public void SetConnectArgumentsContextIn(ReverseCallArgumentsContext context, EmbeddingRegistrationRequest arguments)
        => arguments.CallContext = context;

    /// <inheritdoc/>
    public void SetResponseContextIn(ReverseCallResponseContext context, EmbeddingResponse response)
        => response.CallContext = context;
}

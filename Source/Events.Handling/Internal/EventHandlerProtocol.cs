// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Protobuf.Contracts;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.SDK.Services;
using Dolittle.Services.Contracts;
using Grpc.Core;
using static Dolittle.Runtime.Events.Processing.Contracts.EventHandlers;

namespace Dolittle.SDK.Events.Handling.Internal;

/// <summary>
/// An implementation of <see cref="IAmAReverseCallProtocol{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}" /> for event handlers.
/// </summary>
public class EventHandlerProtocol : IAmAReverseCallProtocol<EventHandlerClientToRuntimeMessage, EventHandlerRuntimeToClientMessage, EventHandlerRegistrationRequest, EventHandlerRegistrationResponse, HandleEventRequest, EventHandlerResponse>
{
    /// <inheritdoc/>
    public AsyncDuplexStreamingCall<EventHandlerClientToRuntimeMessage, EventHandlerRuntimeToClientMessage> Call(Channel channel, CallOptions callOptions)
        => new EventHandlersClient(channel).Connect(callOptions);

    /// <inheritdoc/>
    public EventHandlerClientToRuntimeMessage CreateMessageFrom(EventHandlerRegistrationRequest arguments)
        => new()
            { RegistrationRequest = arguments };

    /// <inheritdoc/>
    public EventHandlerClientToRuntimeMessage CreateMessageFrom(Pong pong)
        => new()
            { Pong = pong };

    /// <inheritdoc/>
    public EventHandlerClientToRuntimeMessage CreateMessageFrom(EventHandlerResponse response)
        => new()
            { HandleResult = response };

    /// <inheritdoc/>
    public EventHandlerRegistrationResponse GetConnectResponseFrom(EventHandlerRuntimeToClientMessage message)
        => message.RegistrationResponse;

    /// <inheritdoc/>
    public Failure GetFailureFromConnectResponse(EventHandlerRegistrationResponse response)
        => response.Failure;

    /// <inheritdoc/>
    public Ping GetPingFrom(EventHandlerRuntimeToClientMessage message)
        => message.Ping;

    /// <inheritdoc/>
    public ReverseCallRequestContext GetRequestContextFrom(HandleEventRequest message)
        => message.CallContext;

    /// <inheritdoc/>
    public HandleEventRequest GetRequestFrom(EventHandlerRuntimeToClientMessage message)
        => message.HandleRequest;

    /// <inheritdoc/>
    public void SetConnectArgumentsContextIn(ReverseCallArgumentsContext context, EventHandlerRegistrationRequest arguments)
        => arguments.CallContext = context;

    /// <inheritdoc/>
    public void SetResponseContextIn(ReverseCallResponseContext context, EventHandlerResponse response)
        => response.CallContext = context;
}
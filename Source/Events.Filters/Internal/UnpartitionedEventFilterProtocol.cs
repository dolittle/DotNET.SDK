// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Protobuf.Contracts;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Services.Contracts;
using Grpc.Core;
using static Dolittle.Runtime.Events.Processing.Contracts.Filters;

namespace Dolittle.SDK.Events.Filters.Internal;

/// <summary>
/// Represents the reverse call protocol for registering unpartitioned event filters with the runtime.
/// </summary>
public class UnpartitionedEventFilterProtocol : IAmAFilterProtocol<FilterClientToRuntimeMessage, FilterRegistrationRequest, FilterResponse>
{
    /// <inheritdoc/>
    public AsyncDuplexStreamingCall<FilterClientToRuntimeMessage, FilterRuntimeToClientMessage> Call(ChannelBase channel, CallOptions callOptions)
        => new FiltersClient(channel).Connect(callOptions);

    /// <inheritdoc/>
    public FilterClientToRuntimeMessage CreateMessageFrom(FilterRegistrationRequest arguments)
        => new() { RegistrationRequest = arguments };

    /// <inheritdoc/>
    public FilterClientToRuntimeMessage CreateMessageFrom(Pong pong)
        => new() { Pong = pong };

    /// <inheritdoc/>
    public FilterClientToRuntimeMessage CreateMessageFrom(FilterResponse response)
        => new() { FilterResult = response };

    /// <inheritdoc/>
    public FilterRegistrationResponse GetConnectResponseFrom(FilterRuntimeToClientMessage message)
        => message.RegistrationResponse;

    /// <inheritdoc/>
    public Failure GetFailureFromConnectResponse(FilterRegistrationResponse response)
        => response.Failure;

    /// <inheritdoc/>
    public Ping GetPingFrom(FilterRuntimeToClientMessage message)
        => message.Ping;

    /// <inheritdoc/>
    public ReverseCallRequestContext GetRequestContextFrom(FilterEventRequest message)
        => message.CallContext;

    /// <inheritdoc/>
    public FilterEventRequest GetRequestFrom(FilterRuntimeToClientMessage message)
        => message.FilterRequest;

    /// <inheritdoc/>
    public void SetConnectArgumentsContextIn(ReverseCallArgumentsContext context, FilterRegistrationRequest arguments)
        => arguments.CallContext = context;

    /// <inheritdoc/>
    public void SetResponseContextIn(ReverseCallResponseContext context, FilterResponse response)
        => response.CallContext = context;
}

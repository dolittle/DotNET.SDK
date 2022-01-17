// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Protobuf.Contracts;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Services.Contracts;
using Grpc.Core;
using static Dolittle.Runtime.Events.Processing.Contracts.Filters;

namespace Dolittle.SDK.Events.Filters.Internal;

/// <summary>
/// Represents the reverse call protocol for registering event filters with the runtime.
/// </summary>
public class PartitionedEventFilterProtocol : IAmAFilterProtocol<PartitionedFilterClientToRuntimeMessage, PartitionedFilterRegistrationRequest, PartitionedFilterResponse>
{
    /// <inheritdoc/>
    public AsyncDuplexStreamingCall<PartitionedFilterClientToRuntimeMessage, FilterRuntimeToClientMessage> Call(Channel channel, CallOptions callOptions)
        => new FiltersClient(channel).ConnectPartitioned(callOptions);

    /// <inheritdoc/>
    public PartitionedFilterClientToRuntimeMessage CreateMessageFrom(PartitionedFilterRegistrationRequest arguments)
        => new() { RegistrationRequest = arguments };

    /// <inheritdoc/>
    public PartitionedFilterClientToRuntimeMessage CreateMessageFrom(Pong pong)
        => new() { Pong = pong };

    /// <inheritdoc/>
    public PartitionedFilterClientToRuntimeMessage CreateMessageFrom(PartitionedFilterResponse response)
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
    public void SetConnectArgumentsContextIn(ReverseCallArgumentsContext context, PartitionedFilterRegistrationRequest arguments)
        => arguments.CallContext = context;

    /// <inheritdoc/>
    public void SetResponseContextIn(ReverseCallResponseContext context, PartitionedFilterResponse response)
        => response.CallContext = context;
}

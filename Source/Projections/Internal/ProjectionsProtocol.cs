// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Protobuf.Contracts;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.SDK.Services;
using Dolittle.Services.Contracts;
using Grpc.Core;
using static Dolittle.Runtime.Events.Processing.Contracts.Projections;

namespace Dolittle.SDK.Projections.Internal
{
    /// <summary>
    /// An implementation of <see cref="IAmAReverseCallProtocol{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}" /> for projections.
    /// </summary>
    public class ProjectionsProtocol : IAmAReverseCallProtocol<ProjectionClientToRuntimeMessage, ProjectionRuntimeToClientMessage, ProjectionRegistrationRequest, ProjectionRegistrationResponse, ProjectionRequest, ProjectionResponse>
    {
        /// <inheritdoc/>
        public AsyncDuplexStreamingCall<ProjectionClientToRuntimeMessage, ProjectionRuntimeToClientMessage> Call(Channel channel, CallOptions callOptions)
            => new ProjectionsClient(channel).Connect(callOptions);

        /// <inheritdoc/>
        public ProjectionClientToRuntimeMessage CreateMessageFrom(ProjectionRegistrationRequest arguments)
            => new ProjectionClientToRuntimeMessage { RegistrationRequest = arguments };

        /// <inheritdoc/>
        public ProjectionClientToRuntimeMessage CreateMessageFrom(Pong pong)
            => new ProjectionClientToRuntimeMessage { Pong = pong };

        /// <inheritdoc/>
        public ProjectionClientToRuntimeMessage CreateMessageFrom(ProjectionResponse response)
            => new ProjectionClientToRuntimeMessage { HandleResult = response };

        /// <inheritdoc/>
        public ProjectionRegistrationResponse GetConnectResponseFrom(ProjectionRuntimeToClientMessage message)
            => message.RegistrationResponse;

        /// <inheritdoc/>
        public Failure GetFailureFromConnectResponse(ProjectionRegistrationResponse response)
            => response.Failure;

        /// <inheritdoc/>
        public Ping GetPingFrom(ProjectionRuntimeToClientMessage message)
            => message.Ping;

        /// <inheritdoc/>
        public ReverseCallRequestContext GetRequestContextFrom(ProjectionRequest message)
            => message.CallContext;

        /// <inheritdoc/>
        public ProjectionRequest GetRequestFrom(ProjectionRuntimeToClientMessage message)
            => message.HandleRequest;

        /// <inheritdoc/>
        public void SetConnectArgumentsContextIn(ReverseCallArgumentsContext context, ProjectionRegistrationRequest arguments)
            => arguments.CallContext = context;

        /// <inheritdoc/>
        public void SetResponseContextIn(ReverseCallResponseContext context, ProjectionResponse response)
            => response.CallContext = context;
    }
}

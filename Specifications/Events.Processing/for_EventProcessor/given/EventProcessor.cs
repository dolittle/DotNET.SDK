// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Protobuf.Contracts;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Services.Clients;
using Moq;

namespace Dolittle.Events.Processing.for_EventProcessor.given
{
    public class EventProcessor : Internal.EventProcessor<EventProcessorId, ClientToRuntimeMessage, RuntimeToClientMessage, ConnectArguments, ConnectResponse, Request, Response>
    {
        readonly bool _sendConnectResponse;
        readonly ConnectArguments _connectArguments;
        readonly ConnectResponse _connectResponse;
        readonly IEnumerable<Request> _requestsToReceive;

        public EventProcessor(EventProcessorId id, bool sendConnectResponse, ConnectArguments connectArguments, ConnectResponse connectResponse, IEnumerable<Request> requestsToReceive)
            : base(new Mock<ILogger>().Object)
        {
            Identifier = id;
            _sendConnectResponse = sendConnectResponse;
            _connectArguments = connectArguments;
            _connectResponse = connectResponse;
            _requestsToReceive = requestsToReceive;
        }

        public IList<ConnectArguments> ConnectArgumentsSent { get; } = new List<ConnectArguments>();

        public IList<Response> ResponsesSent { get; } = new List<Response>();

        protected override string Kind => "specs processor";

        protected override EventProcessorId Identifier { get; }

        protected override IReverseCallClient<ClientToRuntimeMessage, RuntimeToClientMessage, ConnectArguments, ConnectResponse, Request, Response> CreateClient()
        {
            var client = new Mock<IReverseCallClient<ClientToRuntimeMessage, RuntimeToClientMessage, ConnectArguments, ConnectResponse, Request, Response>>();
            client.Setup(_ => _.Connect(Capture.In(ConnectArgumentsSent), It.IsAny<CancellationToken>())).ReturnsAsync(_sendConnectResponse);
            client.SetupGet(_ => _.ConnectResponse).Returns(_connectResponse);
            client.Setup(_ => _.Handle(It.IsAny<Func<Request, CancellationToken, Task<Response>>>(), It.IsAny<CancellationToken>())).Callback(
                (Func<Request, CancellationToken, Task<Response>> handler, CancellationToken cancellationToken) =>
                {
                    foreach (var request in _requestsToReceive)
                    {
                        var response = handler(request, cancellationToken).GetAwaiter().GetResult();
                        ResponsesSent.Add(response);
                    }
                }).Returns(Task.CompletedTask);

            return client.Object;
        }

        protected override ConnectArguments GetRegisterArguments() => _connectArguments;

        protected override Response CreateResponseFromFailure(ProcessorFailure failure) => new Response { Failure = failure };

        protected override Failure GetFailureFromRegisterResponse(ConnectResponse response) => response.Failure;

        protected override RetryProcessingState GetRetryProcessingState(Request request) => request.RetryProcessingState;

        protected override Task<Response> Handle(Request request, CancellationToken cancellationToken) => Task.FromResult(new Response { Request = request });
    }
}
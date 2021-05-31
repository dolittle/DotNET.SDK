// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Protobuf;
using Dolittle.SDK.Security;
using Dolittle.SDK.Services.given.ReverseCall;
using Dolittle.Services.Contracts;
using Machine.Specifications;
using Microsoft.Reactive.Testing;
using Moq;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;
using It = Machine.Specifications.It;
using Version = Dolittle.SDK.Microservices.Version;

namespace Dolittle.SDK.Services.for_ReverseCallClient.when_handling
{
    public class a_request_that_throws_an_error : given.a_reverse_call_client
    {
        static ConnectArguments connectArguments;
        static ConnectResponse connectResponse;
        static ExecutionContext requestExecutionContext;
        static Request request;
        static Exception exception;
        static Mock<IReverseCallHandler<Request, Response>> handler;

        static ExecutionContext executionContextPassedToHandler;
        static ITestableObservable<ServerMessage> serverToClientMessages;
        static ITestableObserver<ConnectResponse> observer;

        Establish context = () =>
        {
            connectArguments = new ConnectArguments();
            connectResponse = new ConnectResponse(connectArguments);

            requestExecutionContext = new ExecutionContext(
                "adf750f8-b3db-413d-b387-8ccda65ae5ac",
                "8bfd7c57-b9cf-4409-aa0a-55130ce0edeb",
                new Version(31, 23, 33, 34, "iprapaponw"),
                "rekfijkuks",
                "abfbd508-497b-4450-b404-991c73eff84f",
                new Claims(new[]
                    {
                        new Claim("pekikilaja", "topvinevki", "daginhadzeoziso"),
                    }),
                CultureInfo.InvariantCulture);

            request = new Request
            {
                Context = new ReverseCallRequestContext
                {
                    CallId = Guid.NewGuid().ToProtobuf(),
                    ExecutionContext = requestExecutionContext.ToProtobuf(),
                }
            };

            exception = new Exception();

            handler = new Mock<IReverseCallHandler<Request, Response>>();
            handler
                .Setup(_ => _.Handle(request, Moq.It.IsAny<ExecutionContext>(), Moq.It.IsAny<CancellationToken>()))
                .Callback<Request, ExecutionContext, CancellationToken>((request, executionContext, token) => executionContextPassedToHandler = executionContext)
                .Returns(Task.FromException<Response>(exception));

            serverToClientMessages = scheduler.CreateHotObservable(
                OnNext(100, new ServerMessage { Response = connectResponse }),
                OnNext(110, new ServerMessage { Request = request }),
                OnCompleted<ServerMessage>(120));
        };

        Because of = () => observer = scheduler.Start(
            () => reverse_call_client_with(connectArguments, handler.Object, serverToClientMessages),
            created: 0,
            subscribed: 0,
            disposed: 1000);

        It should_call_the_handler_to_handle_the_request = () => handler.Verify(_ => _.Handle(request, Moq.It.IsAny<ExecutionContext>(), Moq.It.IsAny<CancellationToken>()));
        It should_not_call_the_handler_any_more = () => handler.VerifyNoOtherCalls();
        It should_pass_the_correct_microservice_to_the_handler = () => executionContextPassedToHandler.Microservice.ShouldEqual(executionContext.Microservice);
        It should_pass_the_correct_tenant_to_the_handler = () => executionContextPassedToHandler.Tenant.ShouldEqual(requestExecutionContext.Tenant);
        It should_pass_the_correct_version_to_the_handler = () => executionContextPassedToHandler.Version.ShouldEqual(executionContext.Version);
        It should_pass_the_correct_environment_to_the_handler = () => executionContextPassedToHandler.Environment.ShouldEqual(executionContext.Environment);
        It should_pass_the_correct_correlation_to_the_handler = () => executionContextPassedToHandler.CorrelationId.ShouldEqual(requestExecutionContext.CorrelationId);
        It should_pass_the_correct_claims_to_the_handler = () => executionContextPassedToHandler.Claims.ShouldEqual(executionContext.Claims);
        It should_pass_the_correct_culture_to_the_handler = () => executionContextPassedToHandler.Culture.ShouldEqual(executionContext.Culture);

        It should_send_the_arguments_and_error_to_the_server = () => ReactiveAssert.AreElementsEqual(
            new[]
            {
                OnNext<ClientMessage>(2, _ => _.Arguments == connectArguments),
                OnError<ClientMessage>(111, _ => _ == exception),
            },
            messagesSentToServer.Messages);

        It should_receive_the_response_and_error = () => ReactiveAssert.AreElementsEqual(
            new[]
            {
                OnNext(100, connectResponse),
                OnError<ConnectResponse>(111, _ => _ == exception),
            },
            observer.Messages);
    }
}
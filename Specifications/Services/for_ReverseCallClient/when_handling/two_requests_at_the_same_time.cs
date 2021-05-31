// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Reactive.Threading.Tasks;
using System.Threading;
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
    public class two_requests_at_the_same_time : given.a_reverse_call_client
    {
        static ConnectArguments connectArguments;
        static ConnectResponse connectResponse;

        static ExecutionContext firstRequestExecutionContext;
        static Request firstRequest;
        static Response firstResponse;

        static ExecutionContext secondRequestExecutionContext;
        static Request secondRequest;
        static Response secondResponse;

        static Mock<IReverseCallHandler<Request, Response>> handler;

        static ExecutionContext firstExecutionContextPassedToHandler;
        static ExecutionContext secondExecutionContextPassedToHandler;
        static ITestableObservable<ServerMessage> serverToClientMessages;
        static ITestableObserver<ConnectResponse> observer;

        Establish context = () =>
        {
            connectArguments = new ConnectArguments();
            connectResponse = new ConnectResponse(connectArguments);

            firstRequestExecutionContext = new ExecutionContext(
                "bdbc69c1-9fe6-407b-9302-7a3b3eca2f2a",
                "1de48dd0-246c-4df1-bf39-c30764b75a6d",
                new Version(1, 1, 1, 1, "first"),
                "first",
                "e114ad78-ed3b-4ace-9460-e4e5c157b175",
                new Claims(new[]
                    {
                        new Claim("first", "first", "first"),
                    }),
                CultureInfo.InvariantCulture);
            firstRequest = new Request
            {
                Context = new ReverseCallRequestContext
                {
                    CallId = Guid.NewGuid().ToProtobuf(),
                    ExecutionContext = firstRequestExecutionContext.ToProtobuf(),
                }
            };
            firstResponse = new Response(firstRequest);

            secondRequestExecutionContext = new ExecutionContext(
                "077e13ff-7897-42d7-a386-9d73ccdf5afd",
                "3d5f6941-f14c-41a2-8476-cc78ba016ae0",
                new Version(1, 1, 1, 1, "second"),
                "second",
                "132829fe-87e3-4156-b6db-38894c75ee3f",
                new Claims(new[]
                    {
                        new Claim("second", "second", "second"),
                    }),
                CultureInfo.InvariantCulture);
            secondRequest = new Request
            {
                Context = new ReverseCallRequestContext
                {
                    CallId = Guid.NewGuid().ToProtobuf(),
                    ExecutionContext = secondRequestExecutionContext.ToProtobuf(),
                }
            };
            secondResponse = new Response(secondRequest);

            var firstSlowResponse = scheduler.CreateHotObservable(
                OnNext(200, firstResponse),
                OnCompleted<Response>(201)).ToTask();
            var secondSlowResponse = scheduler.CreateHotObservable(
                OnNext(140, secondResponse),
                OnCompleted<Response>(141)).ToTask();

            handler = new Mock<IReverseCallHandler<Request, Response>>();
            handler
                .Setup(_ => _.Handle(firstRequest, Moq.It.IsAny<ExecutionContext>(), Moq.It.IsAny<CancellationToken>()))
                .Callback<Request, ExecutionContext, CancellationToken>((request, executionContext, token) => firstExecutionContextPassedToHandler = executionContext)
                .Returns(firstSlowResponse);
            handler
                .Setup(_ => _.Handle(secondRequest, Moq.It.IsAny<ExecutionContext>(), Moq.It.IsAny<CancellationToken>()))
                .Callback<Request, ExecutionContext, CancellationToken>((request, executionContext, token) => secondExecutionContextPassedToHandler = executionContext)
                .Returns(secondSlowResponse);

            serverToClientMessages = scheduler.CreateHotObservable(
                OnNext(100, new ServerMessage { Response = connectResponse }),
                OnNext(110, new ServerMessage { Request = firstRequest }),
                OnNext(120, new ServerMessage { Request = secondRequest }),
                OnCompleted<ServerMessage>(250));
        };

        Because of = () => observer = scheduler.Start(
            () => reverse_call_client_with(connectArguments, handler.Object, serverToClientMessages),
            created: 0,
            subscribed: 0,
            disposed: 1000);

        It should_call_the_handler_to_handle_the_first_request = () => handler.Verify(_ => _.Handle(firstRequest, Moq.It.IsAny<ExecutionContext>(), Moq.It.IsAny<CancellationToken>()));
        It should_call_the_handler_to_handle_the_second_request = () => handler.Verify(_ => _.Handle(secondRequest, Moq.It.IsAny<ExecutionContext>(), Moq.It.IsAny<CancellationToken>()));
        It should_not_call_the_handler_any_more = () => handler.VerifyNoOtherCalls();
        It should_pass_the_correct_tenant_to_the_first_handle = () => firstExecutionContextPassedToHandler.Tenant.ShouldEqual(firstRequestExecutionContext.Tenant);
        It should_pass_the_correct_correlation_to_the_first_handle = () => firstExecutionContextPassedToHandler.CorrelationId.ShouldEqual(firstRequestExecutionContext.CorrelationId);
        It should_pass_the_correct_tenant_to_the_second_handle = () => secondExecutionContextPassedToHandler.Tenant.ShouldEqual(secondRequestExecutionContext.Tenant);
        It should_pass_the_correct_correlation_to_the_second_handle = () => secondExecutionContextPassedToHandler.CorrelationId.ShouldEqual(secondRequestExecutionContext.CorrelationId);

        It should_send_the_arguments_and_the_responses_to_the_server = () => ReactiveAssert.AreElementsEqual(
            new[]
            {
                OnNext<ClientMessage>(2, _ => _.Arguments == connectArguments),
                OnNext<ClientMessage>(142, _ => _.Response == secondResponse && _.Response.Context.CallId == secondRequest.Context.CallId),
                OnNext<ClientMessage>(202, _ => _.Response == firstResponse && _.Response.Context.CallId == firstRequest.Context.CallId),
                OnCompleted<ClientMessage>(250),
            },
            messagesSentToServer.Messages);

        It should_receive_the_response = () => ReactiveAssert.AreElementsEqual(
            new[]
            {
                OnNext(100, connectResponse),
                OnCompleted<ConnectResponse>(250),
            },
            observer.Messages);
    }
}
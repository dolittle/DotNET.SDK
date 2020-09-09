// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Artifacts.given.ReverseCall;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Services;
using Machine.Specifications;
using Microsoft.Extensions.Logging;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.Artifacts.for_Artifacts
{
    public class when_creating
    {
        static ConnectArguments arguments;
        static IReverseCallHandler<Request, Response> handler;
        static ICanCallADuplexStreamingMethod<ClientMessage, ServerMessage> method;
        static IConvertReverseCallMessages<ClientMessage, ServerMessage, ConnectArguments, ConnectResponse, Request, Response> converter;
        static Mock<ILoggerFactory> loggerFactoryMock;
        static ReverseCallClientCreator reverseCallClientCreator;

        static IReverseCallClient<ConnectArguments, ConnectResponse, Request, Response> reverseCallClient;

        Establish context = () =>
        {
            arguments = new ConnectArguments();

            handler = Mock.Of<IReverseCallHandler<Request, Response>>();

            method = Mock.Of<ICanCallADuplexStreamingMethod<ClientMessage, ServerMessage>>();

            converter = Mock.Of<IConvertReverseCallMessages<ClientMessage, ServerMessage, ConnectArguments, ConnectResponse, Request, Response>>();

            loggerFactoryMock = new Mock<ILoggerFactory>();

            reverseCallClientCreator = new ReverseCallClientCreator(
                TimeSpan.FromSeconds(23),
                Mock.Of<IPerformMethodCalls>(),
                Mock.Of<IExecutionContextManager>(),
                loggerFactoryMock.Object);
        };

        Because of = () => reverseCallClient = reverseCallClientCreator.Create(arguments, handler, method, converter);

        It should_create_a_client_with_the_correct_arguments = () => reverseCallClient.Arguments.ShouldEqual(arguments);
        It should_create_a_client_with_the_correct_handler = () => reverseCallClient.Handler.ShouldEqual(handler);
        It should_use_the_logger_factory_to_create_a_new_logger = () => loggerFactoryMock.Verify(_ => _.CreateLogger("Dolittle.SDK.Services.ReverseCallClient"));
        It should_not_use_the_logger_factory_for_anything_else = () => loggerFactoryMock.VerifyNoOtherCalls();
    }
}
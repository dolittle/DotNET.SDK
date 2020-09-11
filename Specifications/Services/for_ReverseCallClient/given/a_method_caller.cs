// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reactive.Subjects;
using Dolittle.SDK.Artifacts.given.ReverseCall;
using Dolittle.SDK.Services;
using Machine.Specifications;
using Moq;
using It = Moq.It;

namespace Dolittle.SDK.Artifacts.for_MethodCaller.given
{
    public class a_method_caller : an_execution_context_manager
    {
        protected static ReplaySubject<ClientMessage> messagesSentToServer;

        Establish context = () => messagesSentToServer = new ReplaySubject<ClientMessage>();

        protected static IPerformMethodCalls MethodCallerThatRepliesWith(IObservable<ServerMessage> serverToClientMessages)
        {
            var mock = new Mock<IPerformMethodCalls>();
            mock.Setup(_ => _.Call(It.IsAny<ICanCallADuplexStreamingMethod<ClientMessage, ServerMessage>>(), It.IsAny<IObservable<ClientMessage>>()))
                .Returns((ICanCallADuplexStreamingMethod<ClientMessage, ServerMessage> method, IObservable<ClientMessage> clientToServerMessages) =>
                    {
                        clientToServerMessages.Subscribe(messagesSentToServer);
                        return serverToClientMessages;
                    });

            return mock.Object;
        }
    }
}
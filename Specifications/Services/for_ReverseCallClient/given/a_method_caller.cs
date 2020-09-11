// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.SDK.Artifacts.given.ReverseCall;
using Dolittle.SDK.Services;
using Machine.Specifications;
using Moq;
using It = Moq.It;

namespace Dolittle.SDK.Artifacts.for_MethodCaller.given
{
    public class a_method_caller : an_execution_context_manager
    {
        protected static IEnumerable<ClientMessage> messagesSentToServer;
        static List<ClientMessage> _messages;

        Establish context = () => messagesSentToServer = _messages = new List<ClientMessage>();

        protected static IPerformMethodCalls MethodCallerThatRepliesWith(IObservable<ServerMessage> serverToClientMessages)
        {
            var mock = new Mock<IPerformMethodCalls>();
            mock.Setup(_ => _.Call(It.IsAny<ICanCallADuplexStreamingMethod<ClientMessage, ServerMessage>>(), It.IsAny<IObservable<ClientMessage>>()))
                .Returns((ICanCallADuplexStreamingMethod<ClientMessage, ServerMessage> method, IObservable<ClientMessage> clientToServerMessages) =>
                    {
                        clientToServerMessages.Subscribe(_messages.Add);
                        return serverToClientMessages;
                    });

            return mock.Object;
        }
    }
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using Dolittle.SDK.Services.given.ReverseCall;
using Machine.Specifications;
using Moq;
using It = Moq.It;

namespace Dolittle.SDK.Services.for_ReverseCallClient.given
{
    public class a_method_caller : an_execution_context_manager
    {
        protected static IEnumerable<ClientMessage> messagesSentToServer;
        static List<ClientMessage> _messages;

        Establish context = () => messagesSentToServer = _messages = new List<ClientMessage>();

        protected static IPerformMethodCalls method_caller_that_replies_with(IObservable<ServerMessage> serverToClientMessages)
        {
            var mock = new Mock<IPerformMethodCalls>();
            mock.Setup(_ => _.Call(It.IsAny<ICanCallADuplexStreamingMethod<ClientMessage, ServerMessage>>(), It.IsAny<IObservable<ClientMessage>>()))
                .Returns((ICanCallADuplexStreamingMethod<ClientMessage, ServerMessage> method, IObservable<ClientMessage> clientToServerMessages)
                    => Observable.Create<ServerMessage>(observer =>
                    {
                        var cts = new CancellationTokenSource();
                        serverToClientMessages.Subscribe(observer, cts.Token);
                        clientToServerMessages.Subscribe(
                            message => _messages.Add(message),
                            error =>
                            {
                                observer.OnError(error);
                                cts.Cancel();
                            },
                            cts.Token);
                        return Disposable.Create(cts.Cancel);
                    }));

            return mock.Object;
        }
    }
}
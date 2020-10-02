// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using Dolittle.SDK.Services.given.ReverseCall;
using Machine.Specifications;
using Microsoft.Reactive.Testing;
using Moq;
using It = Moq.It;

namespace Dolittle.SDK.Services.for_ReverseCallClient.given
{
    public class a_method_caller : an_execution_context
    {
        protected static ITestableObserver<ClientMessage> messagesSentToServer;

        Establish context = () => messagesSentToServer = scheduler.CreateObserver<ClientMessage>();

        protected static IPerformMethodCalls method_caller_that_replies_with(IObservable<ServerMessage> serverToClientMessages)
        {
            var mock = new Mock<IPerformMethodCalls>();
            mock.Setup(_ => _.Call(It.IsAny<ICanCallADuplexStreamingMethod<ClientMessage, ServerMessage>>(), It.IsAny<IObservable<ClientMessage>>(), It.IsAny<CancellationToken>()))
                .Returns((ICanCallADuplexStreamingMethod<ClientMessage, ServerMessage> method, IObservable<ClientMessage> clientToServerMessages, CancellationToken token)
                    => Observable.Create<ServerMessage>(observer =>
                    {
                        var recordingSubscription = clientToServerMessages.Subscribe(messagesSentToServer);
                        var respondingSubscription = serverToClientMessages.Subscribe(observer);
                        var errorSubscription = clientToServerMessages.Subscribe(
                            _ => { },
                            error => observer.OnError(error),
                            () => { });
                        return Disposable.Create(() =>
                            {
                                recordingSubscription.Dispose();
                                respondingSubscription.Dispose();
                                errorSubscription.Dispose();
                            });
                    }));

            return mock.Object;
        }
    }
}
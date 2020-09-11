// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Artifacts.given.ReverseCall;
using Grpc.Core;
using Machine.Specifications;
using Moq;
using It = Moq.It;

namespace Dolittle.SDK.Artifacts.for_MethodCaller.given
{
    public class a_duplex_streaming_method_and_streams : a_duplex_streaming_method
    {
        protected static IList<ClientMessage> writtenClientMessages;
        protected static IClientStreamWriter<ClientMessage> clientStreamWriter;

        Establish context = () =>
        {
            writtenClientMessages = new List<ClientMessage>();

            var mock = new Mock<IClientStreamWriter<ClientMessage>>();
            mock.Setup(_ => _.WriteAsync(It.IsAny<ClientMessage>()))
                .Callback<ClientMessage>(writtenClientMessages.Add)
                .Returns(Task.CompletedTask);

            clientStreamWriter = mock.Object;
        };

        protected static IAsyncStreamReader<ServerMessage> AStreamReaderFrom(IList<ServerMessage> messages)
        {
            var mock = new Mock<IAsyncStreamReader<ServerMessage>>();
            var offset = -1;
            mock.Setup(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns<CancellationToken>(_ =>
                    {
                        if (++offset < messages.Count)
                        {
                            return Task.FromResult(true);
                        }

                        return Task.FromResult(false);
                    });
            mock.SetupGet(_ => _.Current)
                .Returns(() => messages[offset]);

            return mock.Object;
        }
    }
}
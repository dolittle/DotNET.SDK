// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.SDK.Artifacts.given.ReverseCall;
using Dolittle.SDK.Services;
using Grpc.Core;
using Machine.Specifications;
using Moq;
using It = Moq.It;
using Status = Grpc.Core.Status;

namespace Dolittle.SDK.Artifacts.for_MethodCaller.given
{
    public class a_duplex_streaming_method
    {
        protected static Channel providedChannel;
        protected static CallOptions providedCallOptions;

        Establish context = () =>
        {
            providedChannel = null;
            providedCallOptions = new CallOptions(null, null, default, null, null, null);
        };

        protected static ICanCallADuplexStreamingMethod<ClientMessage, ServerMessage> ADuplexStreamingMethodFrom(IClientStreamWriter<ClientMessage> clientStream, IAsyncStreamReader<ServerMessage> serverStream)
        {
            var mock = new Mock<ICanCallADuplexStreamingMethod<ClientMessage, ServerMessage>>();
            mock.Setup(_ => _.Call(It.IsAny<Channel>(), It.IsAny<CallOptions>()))
                .Callback<Channel, CallOptions>((channel, callOptions) =>
                    {
                        providedChannel = channel;
                        providedCallOptions = callOptions;
                    })
                #pragma warning disable CA2000
                .Returns(new AsyncDuplexStreamingCall<ClientMessage, ServerMessage>(
                    clientStream,
                    serverStream,
                    Task.FromResult(new Metadata()),
                    () => new Status(StatusCode.OK, "OK"),
                    () => new Metadata(),
                    () => { }));
                #pragma warning restore CA2000
            return mock.Object;
        }
    }
}
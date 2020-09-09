// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Dolittle.SDK.Artifacts.given.ReverseCall;
using Dolittle.SDK.Artifacts.given.Streams;
using Dolittle.SDK.Services;
using Grpc.Core;

namespace Dolittle.SDK.Artifacts.given
{
    public class Method : ICanCallADuplexStreamingMethod<ClientMessage, ServerMessage>
    {
        readonly List<(Channel, CallOptions)> _calls = new List<(Channel, CallOptions)>();
        readonly List<ClientMessage> _clientToServerMessages = new List<ClientMessage>();
        readonly List<ServerMessage> _serverToClientMessages;

        public Method(IEnumerable<ServerMessage> serverToClientMessages)
        {
            _serverToClientMessages = new List<ServerMessage>(serverToClientMessages);
        }

        public IEnumerable<ClientMessage> ClientToServerMessages => _clientToServerMessages;

        public IEnumerable<ServerMessage> ServerToClientMessages => _serverToClientMessages;

        public IEnumerable<(Channel channel, CallOptions options)> Calls => _calls;

        public AsyncDuplexStreamingCall<ClientMessage, ServerMessage> Call(Channel channel, CallOptions callOptions)
        {
            _calls.Add((channel, callOptions));
            var writer = new Writer(_clientToServerMessages);
            var reader = new Reader(_serverToClientMessages, writer.Completed);
            return new AsyncDuplexStreamingCall<ClientMessage, ServerMessage>(
                writer,
                reader,
                Task.FromResult(new Metadata()),
                () => new Status(StatusCode.OK, "OK"),
                () => new Metadata(),
                () => { });
        }
    }
}
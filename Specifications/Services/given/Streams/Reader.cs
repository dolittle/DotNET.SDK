// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Artifacts.given.ReverseCall;
using Grpc.Core;

namespace Dolittle.SDK.Artifacts.given.Streams
{
    public class Reader : IAsyncStreamReader<ServerMessage>
    {
        readonly IList<ServerMessage> _messages;
        int _offset = -1;

        public Reader(IEnumerable<ServerMessage> messages)
        {
            _messages = new List<ServerMessage>(messages);
        }

        public ServerMessage Current { get; private set; }

        public Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            _offset += 1;
            return Task.FromResult(_offset < _messages.Count);
        }
    }
}
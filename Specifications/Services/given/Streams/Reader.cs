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
        readonly Task _completed;
        int _offset = -1;

        public Reader(IEnumerable<ServerMessage> messages, Task completed)
        {
            _messages = new List<ServerMessage>(messages);
            _completed = completed;
        }

        public ServerMessage Current { get; private set; }

        public async Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            _offset += 1;
            if (_offset < _messages.Count)
            {
                Current = _messages[_offset];
                return true;
            }
            else
            {
                await _completed.ConfigureAwait(false);
                return false;
            }
        }
    }
}
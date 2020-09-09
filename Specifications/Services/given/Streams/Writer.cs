// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dolittle.SDK.Artifacts.given.ReverseCall;
using Grpc.Core;

namespace Dolittle.SDK.Artifacts.given.Streams
{
    public class Writer : IClientStreamWriter<ClientMessage>
    {
        readonly IList<ClientMessage> _messageStore;
        bool _completed = false;

        public Writer(IList<ClientMessage> messageStore)
        {
            _messageStore = messageStore;
        }

        public WriteOptions WriteOptions { get; set; }

        public Task WriteAsync(ClientMessage message)
        {
            if (_completed)
            {
                #pragma warning disable DL0008
                throw new InvalidOperationException("Can't write the message because the call is complete.");
                #pragma warning restore DL0008
            }

            _messageStore.Add(message);
            return Task.CompletedTask;
        }

        public Task CompleteAsync()
        {
            _completed = true;
            return Task.CompletedTask;
        }
    }
}
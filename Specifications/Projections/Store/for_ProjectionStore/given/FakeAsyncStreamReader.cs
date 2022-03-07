// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

namespace Dolittle.SDK.Projections.Store.for_ProjectionStore.given;

public class FakeAsyncStreamReader<TMessage> : IAsyncStreamReader<TMessage>
{
    readonly IEnumerator<TMessage> _messages;

    public FakeAsyncStreamReader(IEnumerable<TMessage> messages)
    {
        _messages = messages.GetEnumerator();
    }
    public FakeAsyncStreamReader(params TMessage[] messages)
    {
        _messages = messages.AsEnumerable().GetEnumerator();
    }

    public Task<bool> MoveNext(CancellationToken cancellationToken)
        => Task.FromResult(!cancellationToken.IsCancellationRequested && _messages.MoveNext());

    public TMessage Current => _messages.Current ?? throw new InvalidOperationException("No current element is available.");
}
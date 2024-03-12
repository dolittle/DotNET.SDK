// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Channels;

namespace Dolittle.SDK.Projections.Store;

public sealed class Subscription<T> : ISubscription<T>
{
    Action? _unsubscribe;

    public Subscription(ChannelReader<T> channel, Action unsubscribe)
    {
        _unsubscribe = unsubscribe;
        Channel = channel;
    }

    /// <inheritdoc />
    public ChannelReader<T> Channel { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        _unsubscribe?.Invoke();
        _unsubscribe = null;
    }
}

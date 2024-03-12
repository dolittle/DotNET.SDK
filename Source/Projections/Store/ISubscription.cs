// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Channels;

namespace Dolittle.SDK.Projections.Store;

public interface ISubscription<T> : IDisposable
{
    /// <summary>
    /// The <see cref="ChannelReader{T}"/> for the subscription.
    /// </summary>
    ChannelReader<T> Channel { get; }
}

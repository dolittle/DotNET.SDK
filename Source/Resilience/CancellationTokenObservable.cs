// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;

namespace Dolittle.SDK.Resilience;

/// <summary>
/// Represents an <see cref="IObservable{T}"/> that completes when a <see cref="CancellationToken"/> is cancelled.
/// </summary>
/// <typeparam name="T">The type of the elements in the sequence.</typeparam>
public class CancellationTokenObservable<T> : IObservable<T>
{
    readonly CancellationToken _token;

    /// <summary>
    /// Initializes a new instance of the <see cref="CancellationTokenObservable{T}"/> class.
    /// </summary>
    /// <param name="token">The <see cref="CancellationToken"/> to wait for.</param>
    public CancellationTokenObservable(CancellationToken token)
    {
        _token = token;
    }

    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<T> observer)
        => _token.Register(() => observer.OnCompleted());
}
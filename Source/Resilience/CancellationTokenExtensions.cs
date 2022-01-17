// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;

namespace Dolittle.SDK.Resilience;

/// <summary>
/// Extension methods for <see cref="CancellationToken"/>.
/// </summary>
public static class CancellationTokenExtensions
{
    /// <summary>
    /// Create an <see cref="IObservable{T}"/> that completes when the <see cref="CancellationToken"/> is cancelled.
    /// </summary>
    /// <param name="token">The <see cref="CancellationToken"/> to wait for.</param>
    /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
    /// <returns>An <see cref="IObservable{T}"/>.</returns>
    public static IObservable<T> ToObservable<T>(this CancellationToken token)
        => new CancellationTokenObservable<T>(token);
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;

namespace Dolittle.SDK.Resilience;

/// <summary>
/// Extension methods for <see cref="IObservable{T}"/>.
/// </summary>
public static class ObservableExtensions
{
    /// <summary>
    /// Repeats the source observable with the provided <see cref="RetryPolicy"/>.
    /// </summary>
    /// <param name="source">Observable sequence to repeat while the policy dictates so or until it successfully terminates.</param>
    /// <param name="policy">The retry policy to apply.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the retries.</param>
    /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
    /// <returns>>An observable sequence producing the elements of the given sequence repeatedly while the policy dictates so or until it terminates successfully.</returns>
    public static IObservable<T> RetryWithPolicy<T>(this IObservable<T> source, RetryPolicy policy, CancellationToken cancellationToken)
        => source.RetryWhen((exceptions) => policy(exceptions).TakeUntil(cancellationToken.ToObservable<Unit>()));
}
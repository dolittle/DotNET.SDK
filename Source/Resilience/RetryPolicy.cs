// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Resilience;

/// <summary>
/// Defines a policy that describes of when to retry observable subscriptions.
/// </summary>
/// <param name="exceptions">An <see cref="IObservable{T}"/> of type <see cref="Exception"/> that emits the errors from the original observable.</param>
/// <returns>An <see cref="IObservable{T}"/> of type <see cref="Exception"/> that triggers the retries.</returns>
/// <remarks>
/// When using a <see cref="RetryPolicy"/>, subscriptions will be retried whenever they are emitted from the returned observable.
/// To stop retrying, the returned observable should complete.
/// </remarks>
public delegate IObservable<Exception> RetryPolicy(IObservable<Exception> exceptions);
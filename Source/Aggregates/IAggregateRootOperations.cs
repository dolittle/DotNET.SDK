// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dolittle.SDK.Aggregates;

/// <summary>
/// Defines a system for working with operations that can be performed on an
/// <see cref="AggregateRoot"/>.
/// </summary>
/// <typeparam name="TAggregate"><see cref="AggregateRoot"/> type.</typeparam>
public interface IAggregateRootOperations<TAggregate>
    where TAggregate : AggregateRoot
{
    /// <summary>
    /// Perform an operation on an <see cref="AggregateRoot"/>.
    /// </summary>
    /// <param name="method"><see cref="Action{T}">Method</see> to perform.</param>
    /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
    /// <returns>The <see cref="Task" /> representing the asynchronous operation.</returns>
    Task Perform(Action<TAggregate> method, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Perform an operation on an <see cref="AggregateRoot"/> and return a value.
    /// </summary>
    /// <param name="method"><see cref="Action{T}">Method</see> to perform.</param>
    /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
    /// <returns>The <see cref="Task{TResponse}" /> representing the asynchronous operation.</returns>
    Task<TResponse> Perform<TResponse>(Func<TAggregate, TResponse> method, CancellationToken cancellationToken = default);

    /// <summary>
    /// Perform an asynchronous operation on an <see cref="AggregateRoot"/>.
    /// </summary>
    /// <param name="method"><see cref="Action{T}">Method</see> to perform.</param>
    /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
    /// <returns>The <see cref="Task" /> representing the asynchronous operation.</returns>
    Task Perform(Func<TAggregate, Task> method, CancellationToken cancellationToken = default);
}

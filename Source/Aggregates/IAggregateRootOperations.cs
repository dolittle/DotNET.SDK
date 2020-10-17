// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Dolittle.SDK.Aggregates
{
    /// <summary>
    /// Defines a system for working with operations that can be performed on an
    /// <see cref="AggregateRoot"/>.
    /// </summary>
    /// <typeparam name="TAggregate"><see cref="AggregateRoot"/> type.</typeparam>
    public interface IAggregateRootOperations<TAggregate>
        where TAggregate : class
    {
        /// <summary>
        /// Perform an operation on an <see cref="AggregateRoot"/>.
        /// </summary>
        /// <param name="method"><see cref="Action{T}">Method</see> to perform.</param>
        /// <returns>The <see cref="Task" /> representing the asynchronous operation.</returns>
        Task Perform(Action<TAggregate> method);
    }
}

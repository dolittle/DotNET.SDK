// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Async;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Aggregates.Internal;

/// <summary>
/// Defines a system that knows how to create aggregate root instances.
/// </summary>
public interface IAggregateRoots
{
    /// <summary>
    /// Tries to get an <typeparamref name="TAggregate"/> instance for the specified <see cref="EventSourceId"/>.
    /// </summary>
    /// <param name="eventSourceId">The <see cref="EventSourceId"/> of the aggregate root instance to create.</param>
    /// <typeparam name="TAggregate"><see cref="AggregateRoot"/> type.</typeparam>
    /// <returns>A <see cref="Try{TResult}"/> with the <typeparamref name="TAggregate"/>.</returns>
    Try<TAggregate> TryGet<TAggregate>(EventSourceId eventSourceId)
        where TAggregate : AggregateRoot;
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Aggregates;

/// <summary>
/// Defines a system that knows how to get the <see cref="IAggregateRootOperations{TAggregate}"/> for a specific <see cref="AggregateRoot"/>
/// scoped to a tenant.
/// </summary>
/// <typeparam name="TAggregateRoot">The <see cref="Type"/> of the <see cref="AggregateRoot"/> class.</typeparam>
public interface IAggregateOf<TAggregateRoot>
    where TAggregateRoot : AggregateRoot
{
    /// <summary>
    /// Gets the <see cref="IAggregateRootOperations{TAggregate}"/> for the <typeparamref name="TAggregateRoot"/> <see cref="AggregateRoot"/> class.
    /// </summary>
    /// <param name="eventSourceId">The <see cref="EventSourceId"/> of the aggregate to get.</param>
    /// <returns>The <see cref="IAggregateRootOperations{TAggregate}"/> for the <typeparamref name="TAggregateRoot"/> <see cref="AggregateRoot"/> class.</returns>
    IAggregateRootOperations<TAggregateRoot> Get(EventSourceId eventSourceId);
}

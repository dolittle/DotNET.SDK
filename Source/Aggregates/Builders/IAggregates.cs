// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Aggregates.Builders;

/// <summary>
/// Defines a system that knows how to get <see cref="IAggregateRootOperations{TAggregate}"/> for aggregates scoped to a tenant.
/// </summary>
public interface IAggregates
{
    /// <summary>
    /// Gets the <see cref="IAggregateRootOperations{TAggregate}"/> for the <typeparamref name="TAggregateRoot"/> <see cref="AggregateRoot"/> class.
    /// </summary>
    /// <param name="eventSourceId">The <see cref="EventSourceId"/> of the aggregate to get.</param>
    /// <typeparam name="TAggregateRoot">The <see cref="Type"/> of the <see cref="AggregateRoot"/> class.</typeparam>
    /// <returns>The <see cref="IAggregateRootOperations{TAggregate}"/> for the <typeparamref name="TAggregateRoot"/> <see cref="AggregateRoot"/> class.</returns>
    IAggregateRootOperations<TAggregateRoot> Get<TAggregateRoot>(EventSourceId eventSourceId)
        where TAggregateRoot : AggregateRoot;

    /// <summary>
    /// Gets the <see cref="IAggregateOf{TAggregate}"/> for the <typeparamref name="TAggregateRoot"/> <see cref="AggregateRoot"/> class.
    /// </summary>
    /// <typeparam name="TAggregateRoot">The <see cref="Type"/> of the <see cref="AggregateRoot"/> class.</typeparam>
    /// <returns>The <see cref="IAggregateOf{TAggregate}"/> for the <typeparamref name="TAggregateRoot"/> <see cref="AggregateRoot"/> class.</returns>
    IAggregateOf<TAggregateRoot> Of<TAggregateRoot>()
        where TAggregateRoot : AggregateRoot;
}

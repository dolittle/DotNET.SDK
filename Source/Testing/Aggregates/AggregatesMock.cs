// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Aggregates.Builders;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Testing.Aggregates;

/// <summary>
/// Represents a mock implementation of <see cref="IAggregates"/>.
/// </summary>
public class AggregatesMock : IAggregates
{
    /// <summary>
    /// Creates a new instance of <see cref="AggregatesMock"/>.
    /// </summary>
    /// <returns>The <see cref="AggregatesMock"/>.</returns>
    public static AggregatesMock Create() => new();
    /// <summary>
    /// Creates a new instance of <see cref="AggregateOfMock{T}"/>.
    /// </summary>
    /// <param name="factory">The factory to create a <typeparamref name="TAggregate"/>.</param>
    /// <typeparam name="TAggregate">The <see cref="Type"/> of the <see cref="AggregateRoot"/>.</typeparam>
    /// <returns>The <see cref="AggregateOfMock{T}"/>.</returns>
    public static AggregateOfMock<TAggregate> Of<TAggregate>(Func<EventSourceId, TAggregate> factory)
        where TAggregate : AggregateRoot => new(factory);
    
    readonly ConcurrentDictionary<Type, object> _aggregatesOf = new();
    readonly ConcurrentDictionary<Type, Func<EventSourceId, object>> _aggregatesFactory = new();

    /// <inheritdoc />
    public IAggregateRootOperations<TAggregateRoot> Get<TAggregateRoot>(EventSourceId eventSourceId)
        where TAggregateRoot : AggregateRoot
    {
        var aggregateOf = GetOrAddAggregateOfMock<TAggregateRoot>();
        return aggregateOf.Get(eventSourceId);
    }

    /// <inheritdoc />
    public IAggregateOf<TAggregateRoot> Of<TAggregateRoot>()
        where TAggregateRoot : AggregateRoot
        => GetOrAddAggregateOfMock<TAggregateRoot>();

    public void WithAggregateFactoryFor<TAggregate>(Func<EventSourceId, TAggregate> factory)
        where TAggregate : AggregateRoot
    {
        _aggregatesFactory[typeof(TAggregate)] = factory;
    }

    AggregateOfMock<TAggregateRoot> GetOrAddAggregateOfMock<TAggregateRoot>()
        where TAggregateRoot : AggregateRoot
    {
        var aggregateRootType = typeof(TAggregateRoot);
        var aggregateOf = _aggregatesOf.GetOrAdd(aggregateRootType, aggregateType =>
        {
            if (!_aggregatesFactory.TryGetValue(aggregateType, out var factory))
            {
                throw new Exception($"No factory to create an aggregate for {aggregateType} has been given to the mock.");
            }
            return new AggregateOfMock<TAggregateRoot>(eventSource => (factory(eventSource) as TAggregateRoot)!);
        });

        return (aggregateOf as AggregateOfMock<TAggregateRoot>)!;
    }
}

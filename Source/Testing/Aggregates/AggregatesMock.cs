// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Aggregates.Builders;
using Dolittle.SDK.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.SDK.Testing.Aggregates;

/// <summary>
/// Represents a mock implementation of <see cref="IAggregates"/>.
/// </summary>
public class AggregatesMock : IAggregates
{
    /// <summary>
    /// Creates a new instance of <see cref="AggregatesMock"/>.
    /// </summary>
    /// <param name="configureServices">The optional callback for configuring the <see cref="IServiceCollection"/> used to create the instances of the aggregate roots.</param>
    /// <returns>The <see cref="AggregatesMock"/>.</returns>
    public static AggregatesMock Create(Action<IServiceCollection>? configureServices = default)
    {
        var services = new ServiceCollection();
        configureServices?.Invoke(services);
        return Create(services.BuildServiceProvider());
    }
    
    /// <summary>
    /// Creates a new instance of <see cref="AggregatesMock"/>.
    /// </summary>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/> used to create the instances of the aggregate roots.</param>
    /// <returns>The <see cref="AggregatesMock"/>.</returns>
    public static AggregatesMock Create(IServiceProvider serviceProvider) => new(serviceProvider);

    readonly ConcurrentDictionary<Type, object> _aggregatesOf = new();

    readonly IServiceProvider _serviceProvider;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="AggregatesMock"/> class.
    /// </summary>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/> used to create the instances of the aggregate roots.</param>
    public AggregatesMock(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

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
    
    /// <summary>
    /// Gets the <see cref="AggregateOfMock{TAggregate}"/>.
    /// </summary>
    /// <typeparam name="TAggregateRoot">The <see cref="Type"/> of the <see cref="AggregateRoot"/>.</typeparam>
    /// <returns>The <see cref="AggregateOfMock{TAggregate}"/>.</returns>
    public AggregateOfMock<TAggregateRoot> OfMock<TAggregateRoot>()
        where TAggregateRoot : AggregateRoot
        => GetOrAddAggregateOfMock<TAggregateRoot>();
    
    AggregateOfMock<TAggregateRoot> GetOrAddAggregateOfMock<TAggregateRoot>()
        where TAggregateRoot : AggregateRoot
    {
        var aggregateRootType = typeof(TAggregateRoot);
        var aggregateOf = _aggregatesOf.GetOrAdd(aggregateRootType, AggregateOfMock<TAggregateRoot>.Create(_serviceProvider));
        return (aggregateOf as AggregateOfMock<TAggregateRoot>)!;
    }
}

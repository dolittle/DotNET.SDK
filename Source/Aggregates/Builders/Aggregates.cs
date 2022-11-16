// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Aggregates.Internal;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Store;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Aggregates.Builders;

/// <summary>
/// Represents an implementation of <see cref="IAggregates"/>.
/// </summary>
public class Aggregates : IAggregates
{
    readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="Aggregates"/> class.
    /// </summary>
    /// <param name="eventStore">The <see cref="IEventStore"/>.</param>
    /// <param name="eventTypes">The <see cref="IEventTypes"/>.</param>
    /// <param name="aggregateRoots">The <see cref="IAggregateRoots"/>.</param>
    /// <param name="serviceProvider">The tenant scoped <see cref="IServiceProvider"/>.</param>
    /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
    public Aggregates(IEventStore eventStore, IEventTypes eventTypes, IAggregateRoots aggregateRoots, IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public IAggregateRootOperations<TAggregateRoot> Get<TAggregateRoot>(EventSourceId eventSourceId)
        where TAggregateRoot : AggregateRoot
        => ActivatorUtilities.CreateInstance<AggregateRootOperations<TAggregateRoot>>(_serviceProvider, eventSourceId);

    /// <inheritdoc />
    public IAggregateOf<TAggregateRoot> Of<TAggregateRoot>()
        where TAggregateRoot : AggregateRoot
        => new AggregateOf<TAggregateRoot>(this);
}

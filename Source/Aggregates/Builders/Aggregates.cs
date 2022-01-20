// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Aggregates.Internal;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Store;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Aggregates.Builders;

/// <summary>
/// Represents an implementation of <see cref="IAggregates"/>.
/// </summary>
public class Aggregates : IAggregates
{
    readonly IEventStore _eventStore;
    readonly IEventTypes _eventTypes;
    readonly IAggregateRoots _aggregateRoots;
    readonly ILoggerFactory _loggerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="Aggregates"/> class.
    /// </summary>
    /// <param name="eventStore">The <see cref="IEventStore"/>.</param>
    /// <param name="eventTypes">The <see cref="IEventTypes"/>.</param>
    /// <param name="aggregateRoots">The <see cref="IAggregateRoots"/>.</param>
    /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
    public Aggregates(IEventStore eventStore, IEventTypes eventTypes, IAggregateRoots aggregateRoots, ILoggerFactory loggerFactory)
    {
        _eventStore = eventStore;
        _eventTypes = eventTypes;
        _aggregateRoots = aggregateRoots;
        _loggerFactory = loggerFactory;
    }

    /// <inheritdoc />
    public IAggregateRootOperations<TAggregateRoot> Get<TAggregateRoot>(EventSourceId eventSourceId)
        where TAggregateRoot : AggregateRoot
        => new AggregateRootOperations<TAggregateRoot>(
            eventSourceId,
            _eventStore,
            _eventTypes,
            _aggregateRoots,
            _loggerFactory.CreateLogger<AggregateRootOperations<TAggregateRoot>>());

    /// <inheritdoc />
    public IAggregateOf<TAggregateRoot> Of<TAggregateRoot>()
        where TAggregateRoot : AggregateRoot
        => new AggregateOf<TAggregateRoot>(this);
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.SDK.Events.Store.Builders;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Store;

/// <summary>
/// Represents an implementation of <see cref="ICommitAggregateEvents" />.
/// </summary>
public class AggregateEventCommitter : ICommitAggregateEvents
{
    readonly Internal.ICommitAggregateEvents _aggregateEvents;
    readonly IEventTypes _eventTypes;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateEventCommitter"/> class.
    /// </summary>
    /// <param name="aggregateEvents">The <see cref="Internal.ICommitAggregateEvents" />.</param>
    /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
    /// <param name="logger">The <see cref="ILogger" />.</param>
    public AggregateEventCommitter(
        Internal.ICommitAggregateEvents aggregateEvents,
        IEventTypes eventTypes,
        ILogger logger)
    {
        _aggregateEvents = aggregateEvents;
        _eventTypes = eventTypes;
        _logger = logger;
    }

    /// <inheritdoc/>
    public CommitForAggregateBuilder ForAggregate(AggregateRootId aggregateRootId, CancellationToken cancellationToken = default)
        => new(_aggregateEvents, _eventTypes, aggregateRootId, _logger);
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.SDK.Events.Store.Builders;

namespace Dolittle.SDK.Events.Store;

/// <summary>
/// Represents an implementation of <see cref="ICommitAggregateEvents" />.
/// </summary>
public class AggregateEventCommitter : ICommitAggregateEvents
{
    readonly Internal.ICommitAggregateEvents _aggregateEvents;
    readonly IEventTypes _eventTypes;

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateEventCommitter"/> class.
    /// </summary>
    /// <param name="aggregateEvents">The <see cref="Internal.ICommitAggregateEvents" />.</param>
    /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
    public AggregateEventCommitter(
        Internal.ICommitAggregateEvents aggregateEvents,
        IEventTypes eventTypes)
    {
        _aggregateEvents = aggregateEvents;
        _eventTypes = eventTypes;
    }

    /// <inheritdoc/>
    public CommitForAggregateBuilder ForAggregate(AggregateRootId aggregateRootId, CancellationToken cancellationToken = default)
        => new(_aggregateEvents, _eventTypes, aggregateRootId);
}

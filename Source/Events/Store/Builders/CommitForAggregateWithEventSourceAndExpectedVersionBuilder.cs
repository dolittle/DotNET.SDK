// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Events.Builders;

namespace Dolittle.SDK.Events.Store.Builders;

/// <summary>
/// Represents a builder for an aggregate events commit.
/// </summary>
public class CommitForAggregateWithEventSourceAndExpectedVersionBuilder
{
    readonly Internal.ICommitAggregateEvents _aggregateEvents;
    readonly IEventTypes _eventTypes;
    readonly AggregateRootId _aggregateRootId;
    readonly EventSourceId _eventSourceId;
    readonly AggregateRootVersion _expectedVersion;
    UncommittedAggregateEventsBuilder _builder;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommitForAggregateWithEventSourceAndExpectedVersionBuilder"/> class.
    /// </summary>
    /// <param name="aggregateEvents">The <see cref="Internal.ICommitAggregateEvents" />.</param>
    /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
    /// <param name="aggregateRootId">The <see cref="AggregateRootId" />.</param>
    /// <param name="eventSourceId">The <see cref="EventSourceId" />.</param>
    /// <param name="expectedVersion">The expected <see cref="AggregateRootVersion" />.</param>
    public CommitForAggregateWithEventSourceAndExpectedVersionBuilder(
        Internal.ICommitAggregateEvents aggregateEvents,
        IEventTypes eventTypes,
        AggregateRootId aggregateRootId,
        EventSourceId eventSourceId,
        AggregateRootVersion expectedVersion)
    {
        _aggregateEvents = aggregateEvents;
        _eventTypes = eventTypes;
        _aggregateRootId = aggregateRootId;
        _eventSourceId = eventSourceId;
        _expectedVersion = expectedVersion;
    }

    /// <summary>
    /// Commit the aggregate events.
    /// </summary>
    /// <param name="callback">The callback to create the <see cref="UncommittedAggregateEvents" />.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="CommittedAggregateEvents" />.</returns>
    public Task<CommittedAggregateEvents> Commit(
        Action<UncommittedAggregateEventsBuilder> callback,
        CancellationToken cancellationToken = default)
    {
        if (_builder != default)
        {
            throw new EventBuilderMethodAlreadyCalled("Commit");
        }
        _builder = new UncommittedAggregateEventsBuilder(_aggregateRootId, _eventSourceId, _expectedVersion);
        callback(_builder);
        var uncommittedAggregateEvents = _builder.Build(_eventTypes);
        return _aggregateEvents.CommitForAggregate(uncommittedAggregateEvents, cancellationToken);
    }
}

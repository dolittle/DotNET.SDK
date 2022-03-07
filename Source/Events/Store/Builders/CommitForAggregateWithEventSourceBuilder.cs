// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events.Builders;

namespace Dolittle.SDK.Events.Store.Builders;

/// <summary>
/// Represents a builder for <see cref="UncommittedAggregateEvents" /> with <see cref="EventSourceId" />.
/// </summary>
public class CommitForAggregateWithEventSourceBuilder
{
    readonly AggregateRootId _aggregateRootId;
    readonly EventSourceId _eventSourceId;
    readonly Internal.ICommitAggregateEvents _aggregateEvents;
    readonly IEventTypes _eventTypes;
    CommitForAggregateWithEventSourceAndExpectedVersionBuilder _builder;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommitForAggregateWithEventSourceBuilder"/> class.
    /// </summary>
    /// <param name="aggregateEvents">The <see cref="Internal.ICommitAggregateEvents" />.</param>
    /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
    /// <param name="aggregateRootId">The <see cref="AggregateRootId" />.</param>
    /// <param name="eventSourceId">The <see cref="EventSourceId" />.</param>
    public CommitForAggregateWithEventSourceBuilder(
        Internal.ICommitAggregateEvents aggregateEvents,
        IEventTypes eventTypes,
        AggregateRootId aggregateRootId,
        EventSourceId eventSourceId)
    {
        _aggregateEvents = aggregateEvents;
        _eventTypes = eventTypes;
        _aggregateRootId = aggregateRootId;
        _eventSourceId = eventSourceId;
    }

    /// <summary>
    /// Configure the expected <see cref="AggregateRootVersion" /> for the <see cref="UncommittedAggregateEvents" />.
    /// </summary>
    /// <param name="expectedVersion">The expected <see cref="AggregateRootVersion" />.</param>
    /// <returns>The <see cref="CommitForAggregateWithEventSourceAndExpectedVersionBuilder" /> for continuation.</returns>
    public CommitForAggregateWithEventSourceAndExpectedVersionBuilder ExpectVersion(AggregateRootVersion expectedVersion)
    {
        if (_builder != default) throw new EventBuilderMethodAlreadyCalled("ExpectVersion");
        _builder = new CommitForAggregateWithEventSourceAndExpectedVersionBuilder(
            _aggregateEvents,
            _eventTypes,
            _aggregateRootId,
            _eventSourceId,
            expectedVersion);
        return _builder;
    }
}

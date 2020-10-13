// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events.Builders;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Store.Builders
{
    /// <summary>
    /// Represents a builder for an aggregate event commit.
    /// </summary>
    public class CommitForAggregateBuilder
    {
        readonly Internal.ICommitAggregateEvents _aggregateEvents;
        readonly IEventTypes _eventTypes;
        readonly AggregateRootId _aggregateRootId;
        readonly ILogger _logger;
        CommitForAggregateWithEventSourceBuilder _builder;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommitForAggregateBuilder"/> class.
        /// </summary>
        /// <param name="aggregateEvents">The <see cref="Internal.ICommitAggregateEvents" />.</param>
        /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
        /// <param name="aggregateRootId">The <see cref="AggregateRootId" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public CommitForAggregateBuilder(
            Internal.ICommitAggregateEvents aggregateEvents,
            IEventTypes eventTypes,
            AggregateRootId aggregateRootId,
            ILogger logger)
        {
            _aggregateEvents = aggregateEvents;
            _eventTypes = eventTypes;
            _aggregateRootId = aggregateRootId;
            _logger = logger;
        }

        /// <summary>
        /// Build aggregate events with event source id.
        /// </summary>
        /// <param name="eventSourceId">The <see cref="EventSourceId" />.</param>
        /// <returns>The <see cref="UncommittedEventBuilder" /> for continuation.</returns>
        public CommitForAggregateWithEventSourceBuilder WithEventSource(EventSourceId eventSourceId)
        {
            if (_builder != default) throw new EventBuilderMethodAlreadyCalled("WithEventSource");
            _builder = new CommitForAggregateWithEventSourceBuilder(_aggregateEvents, _eventTypes, _aggregateRootId, eventSourceId, _logger);
            return _builder;
        }
    }
}

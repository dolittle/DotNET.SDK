// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Events.Builders
{
    /// <summary>
    /// Represents a builder for <see cref="UncommittedAggregateEvents" /> with <see cref="AggregateRootVersion" />.
    /// </summary>
    public class UncommittedAggregateEventsWithAggregateRootBuilder
    {
        readonly AggregateRootId _aggregateRootId;
        UncommittedAggregateEventsWithEventSourceBuilder _builder;

        /// <summary>
        /// Initializes a new instance of the <see cref="UncommittedAggregateEventsWithAggregateRootBuilder"/> class.
        /// </summary>
        /// <param name="aggregateRootId">The <see cref="AggregateRootId" />.</param>
        public UncommittedAggregateEventsWithAggregateRootBuilder(AggregateRootId aggregateRootId) => _aggregateRootId = aggregateRootId;

        /// <summary>
        /// Build aggregate events with event source id.
        /// </summary>
        /// <param name="eventSourceId">The <see cref="EventSourceId" />.</param>
        /// <returns>The <see cref="UncommittedEventBuilder" /> for continuation.</returns>
        public UncommittedAggregateEventsWithEventSourceBuilder WithEventSource(EventSourceId eventSourceId)
        {
            if (_builder != default) throw new EventBuilderMethodAlreadyCalled("WithEventSource");
            _builder = new UncommittedAggregateEventsWithEventSourceBuilder(_aggregateRootId, eventSourceId);
            return _builder;
        }

        /// <summary>
        /// Builds the uncommitted aggregate events.
        /// </summary>
        /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
        /// <returns>The <see cref="UncommittedEvents" />.</returns>
        public UncommittedAggregateEvents Build(IEventTypes eventTypes)
        {
            if (_builder == default) throw new EventDefinitionIncomplete("EventSource", "Call WithEventSource()");
            return _builder.Build(eventTypes);
        }
    }
}

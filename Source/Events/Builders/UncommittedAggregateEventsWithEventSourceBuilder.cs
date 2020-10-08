// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Events.Builders
{
    /// <summary>
    /// Represents a builder for <see cref="UncommittedAggregateEvents" /> with <see cref="EventSourceId" />.
    /// </summary>
    public class UncommittedAggregateEventsWithEventSourceBuilder
    {
        readonly AggregateRootId _aggregateRootId;
        readonly EventSourceId _eventSourceId;
        UncommittedAggregateEventsBuilder _builder;

        /// <summary>
        /// Initializes a new instance of the <see cref="UncommittedAggregateEventsWithEventSourceBuilder"/> class.
        /// </summary>
        /// <param name="aggregateRootId">The <see cref="AggregateRootId" />.</param>
        /// <param name="eventSourceId">The <see cref="EventSourceId" />.</param>
        public UncommittedAggregateEventsWithEventSourceBuilder(AggregateRootId aggregateRootId, EventSourceId eventSourceId)
        {
            _aggregateRootId = aggregateRootId;
            _eventSourceId = eventSourceId;
        }

        /// <summary>
        /// Configure the expected <see cref="AggregateRootVersion" /> for the <see cref="UncommittedAggregateEvents" />.
        /// </summary>
        /// <param name="expectedVersion">The expected <see cref="AggregateRootVersion" />.</param>
        /// <param name="callback">The callback for creating the events.</param>
        public void ExpectVersion(AggregateRootVersion expectedVersion, Action<UncommittedAggregateEventsBuilder> callback)
        {
            if (_builder != default) throw new EventBuilderMethodAlreadyCalled("ExpectVersion");
            _builder = new UncommittedAggregateEventsBuilder(_aggregateRootId, _eventSourceId, expectedVersion);
            callback(_builder);
        }

        /// <summary>
        /// Build <see cref="UncommittedAggregateEvents" />.
        /// </summary>
        /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
        /// <returns>The <see cref="UncommittedAggregateEvents" />.</returns>
        public UncommittedAggregateEvents Build(IEventTypes eventTypes)
        {
            if (_builder == default) throw new EventDefinitionIncomplete("Expected AggregateRootVersion", "Call ExpectVersion() with the expected aggregate root version");
            return _builder.Build(eventTypes);
        }
    }
}
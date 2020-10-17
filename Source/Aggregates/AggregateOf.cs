// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Store;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Aggregates
{
    /// <summary>
    /// Represents an implementation of <see cref="IAggregateOf{T}"/>.
    /// </summary>
    /// <typeparam name="TAggregateRoot">Type of <see cref="AggregateRoot"/>.</typeparam>
    public class AggregateOf<TAggregateRoot> : IAggregateOf<TAggregateRoot>
        where TAggregateRoot : AggregateRoot
    {
        readonly IEventStore _eventStore;
        readonly IAggregateRootFactory _aggregateRootFactory;
        readonly ILoggerFactory _loggerFactory;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateOf{T}"/> class.
        /// </summary>
        /// <param name="eventStore">The <see cref="IEventStore" />.</param>
        /// <param name="aggregateRootFactory">The <see cref="IAggregateRootFactory" />.</param>
        /// <param name="loggerFactory">The <see cref="ILogger" />.</param>
        public AggregateOf(IEventStore eventStore, IAggregateRootFactory aggregateRootFactory, ILoggerFactory loggerFactory)
        {
            _eventStore = eventStore;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<AggregateOf<TAggregateRoot>>();
            _aggregateRootFactory = aggregateRootFactory;
        }

        /// <inheritdoc/>
        public IAggregateRootOperations<TAggregateRoot> Create()
            => Get(EventSourceId.New());

        /// <inheritdoc/>
        public IAggregateRootOperations<TAggregateRoot> Get(EventSourceId eventSourceId)
        {
            if (TryGetAggregateRoot(eventSourceId, out var aggregateRoot, out var exception))
            {
                ReApplyEvents(aggregateRoot);
                return new AggregateRootOperations<TAggregateRoot>(
                    _eventStore,
                    aggregateRoot,
                    _loggerFactory.CreateLogger<AggregateRootOperations<TAggregateRoot>>());
            }

            throw new CouldNotGetAggregateRoot(typeof(TAggregateRoot), eventSourceId, exception.Message);
        }

        bool TryGetAggregateRoot(EventSourceId eventSourceId, out TAggregateRoot aggregateRoot, out Exception exception)
        {
            try
            {
                exception = default;
                _logger.LogDebug(
                    "Getting aggregate root {AggregateRoot} with event source id {EventSource}",
                    typeof(TAggregateRoot),
                    eventSourceId);
                aggregateRoot = _aggregateRootFactory.Create<TAggregateRoot>(eventSourceId);
                return true;
            }
            catch (Exception ex)
            {
                aggregateRoot = default;
                exception = ex;
                return false;
            }
        }

        void ReApplyEvents(TAggregateRoot aggregateRoot)
        {
            var eventSourceId = aggregateRoot.EventSourceId;
            var aggregateRootId = aggregateRoot.GetAggregateRootId();
            _logger.LogDebug(
                "Re-applying events for {AggregateRoot} with aggregate root id {AggregateRootId} with event source id {EventSourceId}",
                typeof(TAggregateRoot),
                aggregateRootId,
                eventSourceId);

            var committedEvents = _eventStore.FetchForAggregate(aggregateRootId, eventSourceId, CancellationToken.None).GetAwaiter().GetResult();
            if (committedEvents.HasEvents)
            {
                _logger.LogTrace("Re-applying {NumberOfEvents} events", committedEvents.Count);
                aggregateRoot.ReApply(committedEvents);
            }
            else
            {
                _logger.LogTrace("No events to re-apply");
            }
        }
    }
}

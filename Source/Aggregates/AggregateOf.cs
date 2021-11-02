// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Aggregates.Internal;
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
        readonly IEventTypes _eventTypes;
        readonly IAggregateRoots _aggregateRoots;
        readonly ILoggerFactory _loggerFactory;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateOf{T}"/> class.
        /// </summary>
        /// <param name="eventStore">The <see cref="IEventStore" />.</param>
        /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
        /// <param name="aggregateRoots">The <see cref="IAggregateRoots"/>.</param>
        /// <param name="loggerFactory">The <see cref="ILogger" />.</param>
        public AggregateOf(IEventStore eventStore, IEventTypes eventTypes, IAggregateRoots aggregateRoots, ILoggerFactory loggerFactory)
        {
            _eventTypes = eventTypes;
            _aggregateRoots = aggregateRoots;
            _eventStore = eventStore;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<AggregateOf<TAggregateRoot>>();
        }

        /// <inheritdoc/>
        public IAggregateRootOperations<TAggregateRoot> Create()
            => Get(EventSourceId.New());

        /// <inheritdoc/>
        public IAggregateRootOperations<TAggregateRoot> Get(EventSourceId eventSourceId)
            => new AggregateRootOperations<TAggregateRoot>(
                eventSourceId,
                _eventStore,
                _eventTypes,
                _aggregateRoots,
                _loggerFactory.CreateLogger<AggregateRootOperations<TAggregateRoot>>());
    }
}

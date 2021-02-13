// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reflection;
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
        readonly IEventTypes _eventTypes;
        readonly ILoggerFactory _loggerFactory;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateOf{T}"/> class.
        /// </summary>
        /// <param name="eventStore">The <see cref="IEventStore" />.</param>
        /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
        /// <param name="loggerFactory">The <see cref="ILogger" />.</param>
        public AggregateOf(IEventStore eventStore, IEventTypes eventTypes, ILoggerFactory loggerFactory)
        {
            _eventTypes = eventTypes;
            _eventStore = eventStore;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<AggregateOf<TAggregateRoot>>();
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
                    _eventTypes,
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
                aggregateRoot = CreateAggregateRoot(eventSourceId);
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

        TAggregateRoot CreateAggregateRoot(EventSourceId eventSourceId)
        {
            var aggregateRootType = typeof(TAggregateRoot);
            ThrowIfInvalidConstructor(aggregateRootType);
            var constructor = typeof(TAggregateRoot).GetConstructors().Single();

            var aggregateRoot = GetInstanceFrom(eventSourceId, constructor);
            ThrowIfCouldNotCreateAggregateRoot(aggregateRoot);
            return aggregateRoot;
        }

        TAggregateRoot GetInstanceFrom(EventSourceId id, ConstructorInfo constructor)
            => constructor.Invoke(
                new object[]
                {
                    id
                }) as TAggregateRoot;

        void ThrowIfInvalidConstructor(Type type)
        {
            ThrowIfNotOneConstructor(type);
            ThrowIfConstructorIsInvalid(type, type.GetConstructors().Single());
        }

        void ThrowIfNotOneConstructor(Type type)
        {
            if (type.GetConstructors().Length != 1)
                throw new InvalidAggregateRootConstructorSignature(type, "expected only a single constructor");
        }

        void ThrowIfConstructorIsInvalid(Type type, ConstructorInfo constructor)
        {
            var parameters = constructor.GetParameters();
            ThrowIfIncorrectNumberOfParameter(type, parameters);
            ThrowIfIncorrectParameter(type, parameters);
        }

        void ThrowIfIncorrectParameter(Type type, ParameterInfo[] parameters)
        {
            if (parameters.Length != 1 ||
                parameters[0].ParameterType != typeof(Guid) ||
                parameters[0].ParameterType != typeof(EventSourceId))
            {
                throw new InvalidAggregateRootConstructorSignature(type, $"expected only one parameter and it must be of type {typeof(Guid)} or {typeof(EventSourceId)}");
            }
        }

        void ThrowIfIncorrectNumberOfParameter(Type type, ParameterInfo[] parameters)
        {
            const int expectedNumberParameters = 1;
            if (parameters.Length != expectedNumberParameters)
                throw new InvalidAggregateRootConstructorSignature(type, $"expected {expectedNumberParameters} parameter");
        }

        void ThrowIfCouldNotCreateAggregateRoot(TAggregateRoot aggregateRoot)
        {
            if (aggregateRoot == default) throw new CouldNotCreateAggregateRootInstance(typeof(TAggregateRoot));
        }
    }
}

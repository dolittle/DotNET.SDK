// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Aggregates.Internal;
using Dolittle.SDK.DependencyInversion;
using Dolittle.SDK.Embeddings;
using Dolittle.SDK.Embeddings.Builder;
using Dolittle.SDK.EventHorizon;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Filters;
using Dolittle.SDK.Events.Handling.Builder;
using Dolittle.SDK.Events.Processing;
using Dolittle.SDK.Events.Store;
using Dolittle.SDK.Events.Store.Builders;
using Dolittle.SDK.Events.Store.Converters;
using Dolittle.SDK.Microservices;
using Dolittle.SDK.Projections.Builder;
using Dolittle.SDK.Projections.Store.Builders;
using Dolittle.SDK.Projections.Store.Converters;
using Dolittle.SDK.Services;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK
{
    /// <summary>
    /// Represents the client for working with the Dolittle Runtime.
    /// </summary>
    public class Client : IDisposable
    {
        readonly ProcessingCoordinator _processingCoordinator;
        readonly EventHandlersBuilder _eventHandlersBuilder;
        readonly EventFiltersBuilder _filtersBuilder;
        readonly IConvertEventsToProtobuf _eventsToProtobufConverter;
        readonly IConvertProjectionsToSDK _projectionConverter;
        readonly ProjectionsBuilder _projectionsBuilder;
        readonly EmbeddingsBuilder _embeddingsBuilder;
        readonly EventHorizons _eventHorizons;
        readonly IEventProcessors _eventProcessors;
        readonly IEventProcessingConverter _eventProcessingConverter;
        readonly IAggregateRoots _aggregateRoots;
        readonly ILoggerFactory _loggerFactory;
        readonly CancellationToken _cancellation;
        IContainer _container;
        bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="Client" /> class.
        /// </summary>
        /// <param name="eventTypes">The <see cref="EventTypes" />.</param>
        /// <param name="eventStoreBuilder">The <see cref="EventStoreBuilder" />.</param>
        /// <param name="eventHorizons">The <see cref="EventHorizons" />.</param>
        /// <param name="processingCoordinator">The <see cref="ProcessingCoordinator" />.</param>
        /// <param name="eventProcessors">The <see cref="IEventProcessors" />.</param>
        /// <param name="eventProcessingConverter">The <see cref="IEventProcessingConverter" />.</param>
        /// <param name="eventHandlersBuilder">The <see cref="EventHandlersBuilder" />.</param>
        /// <param name="filtersBuilder">The <see cref="EventFiltersBuilder" />.</param>
        /// <param name="projectionConverter">The <see cref="IConvertProjectionsToSDK" />.</param>
        /// <param name="eventsToProtobufConverter">The <see cref="IConvertEventsToProtobuf" />.</param>
        /// <param name="projectionsBuilder">The <see cref="ProjectionsBuilder" />.</param>
        /// <param name="embeddingsBuilder">The <see cref="EmbeddingsBuilder" />.</param>
        /// <param name="projectionStoreBuilder">The <see cref="ProjectionStoreBuilder" />.</param>
        /// <param name="embeddings">The <see cref="IEmbeddings" />.</param>
        /// <param name="aggregateRoots">The <see cref="IAggregateRoots"/>.</param>
        /// <param name="tenants">The <see cref="ITenants"/>.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        public Client(
            IEventTypes eventTypes,
            EventStoreBuilder eventStoreBuilder,
            EventHorizons eventHorizons,
            ProcessingCoordinator processingCoordinator,
            IEventProcessors eventProcessors,
            IEventProcessingConverter eventProcessingConverter,
            EventHandlersBuilder eventHandlersBuilder,
            EventFiltersBuilder filtersBuilder,
            IConvertProjectionsToSDK projectionConverter,
            IConvertEventsToProtobuf eventsToProtobufConverter,
            ProjectionsBuilder projectionsBuilder,
            EmbeddingsBuilder embeddingsBuilder,
            ProjectionStoreBuilder projectionStoreBuilder,
            IEmbeddings embeddings,
            IAggregateRoots aggregateRoots,
            ITenants tenants,
            ILoggerFactory loggerFactory,
            CancellationToken cancellationToken)
        {
            EventTypes = eventTypes;
            EventStore = eventStoreBuilder;
            _eventHorizons = eventHorizons;
            _processingCoordinator = processingCoordinator;
            _eventProcessors = eventProcessors;
            _eventProcessingConverter = eventProcessingConverter;
            _eventHandlersBuilder = eventHandlersBuilder;
            _filtersBuilder = filtersBuilder;
            _projectionConverter = projectionConverter;
            _eventsToProtobufConverter = eventsToProtobufConverter;
            _projectionsBuilder = projectionsBuilder;
            _embeddingsBuilder = embeddingsBuilder;
            Projections = projectionStoreBuilder;
            Embeddings = embeddings;
            _aggregateRoots = aggregateRoots;
            Tenants = tenants;
            _loggerFactory = loggerFactory;
            _cancellation = cancellationToken;
            _container = new DefaultContainer();
        }

        /// <summary>
        /// Gets the <see cref="IEventTypes" />.
        /// </summary>
        public IEventTypes EventTypes { get; }

        /// <summary>
        /// Gets the <see cref="EventStoreBuilder" />.
        /// </summary>
        public EventStoreBuilder EventStore { get; }

        /// <summary>
        /// Gets the <see cref="ProjectionStoreBuilder" />.
        /// </summary>
        public ProjectionStoreBuilder Projections { get; }

        /// <summary>
        /// Gets the <see cref="IEmbeddings" />.
        /// </summary>
        public IEmbeddings Embeddings { get; }

        /// <summary>
        /// Gets the <see cref="IEventHorizons" />.
        /// </summary>
        public IEventHorizons EventHorizons => _eventHorizons;

        /// <summary>
        /// Gets the <see cref="ITenants"/>.
        /// </summary>
        public ITenants Tenants { get; }

        /// <summary>
        /// Create a client builder for a Microservice.
        /// </summary>
        /// <param name="microserviceId">The unique identifier for the microservice.</param>
        /// <returns>The <see cref="ClientBuilder"/> to build the <see cref="Client"/> from.</returns>
        public static ClientBuilder ForMicroservice(MicroserviceId microserviceId)
            => new ClientBuilder(microserviceId);

        /// <summary>
        /// /// Sets the <see cref="IContainer" /> to use for inversion of control.
        /// </summary>
        /// <param name="container">The <see cref="IContainer" /> to use for inversion of control.</param>
        /// <returns>The client builder for continuation.</returns>
        public Client WithContainer(IContainer container)
        {
            _container = container;
            return this;
        }

        /// <summary>
        /// Start the client.
        /// </summary>
        /// <returns>A <see cref="Task"/> that completes when the client has started. </returns>
        public Task Start()
        {
            _eventHandlersBuilder.BuildAndRegister(
                _eventProcessors,
                EventTypes,
                _eventProcessingConverter,
                _container,
                _loggerFactory,
                _cancellation);
            _filtersBuilder.BuildAndRegister(
                _eventProcessors,
                _eventProcessingConverter,
                _loggerFactory,
                _cancellation);
            _projectionsBuilder.BuildAndRegister(
                _eventProcessors,
                EventTypes,
                _eventProcessingConverter,
                _projectionConverter,
                _loggerFactory,
                _cancellation);
            _embeddingsBuilder.BuildAndRegister(
                _eventProcessors,
                EventTypes,
                _eventsToProtobufConverter,
                _projectionConverter,
                _loggerFactory,
                _cancellation);
            return _processingCoordinator.Completion;
        }

        /// <summary>
        /// Gets the <see cref="IAggregateRootOperations{TAggregate}" /> for a new aggregate of the specificied <typeparamref name="TAggregateRoot"/>.
        /// </summary>
        /// <param name="buildEventStore">The <see cref="Func{T, TResult}" /> for creating the <see cref="IEventStore" />.</param>
        /// <typeparam name="TAggregateRoot">The <see cref="Type" /> of the <see cref="AggregateRoot" />.</typeparam>
        /// <returns>The <see cref="IAggregateRootOperations{TAggregate}" />.</returns>
        public IAggregateRootOperations<TAggregateRoot> AggregateOf<TAggregateRoot>(Func<EventStoreBuilder, IEventStore> buildEventStore)
            where TAggregateRoot : AggregateRoot
            => new AggregateOf<TAggregateRoot>(buildEventStore(EventStore), EventTypes, _aggregateRoots, _loggerFactory)
                    .Create();

        /// <summary>
        /// Gets the <see cref="IAggregateRootOperations{TAggregate}" /> for a new aggregate of the specificied <typeparamref name="TAggregateRoot"/>.
        /// </summary>
        /// <param name="eventSource">The <see cref="EventSourceId" />.</param>
        /// <param name="buildEventStore">The <see cref="Func{T, TResult}" /> for creating the <see cref="IEventStore" />.</param>
        /// <typeparam name="TAggregateRoot">The <see cref="Type" /> of the <see cref="AggregateRoot" />.</typeparam>
        /// <returns>The <see cref="IAggregateRootOperations{TAggregate}" />.</returns>
        public IAggregateRootOperations<TAggregateRoot> AggregateOf<TAggregateRoot>(EventSourceId eventSource, Func<EventStoreBuilder, IEventStore> buildEventStore)
            where TAggregateRoot : AggregateRoot
            => new AggregateOf<TAggregateRoot>(buildEventStore(EventStore), EventTypes, _aggregateRoots, _loggerFactory)
                    .Get(eventSource);

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose resources.
        /// </summary>
        /// <param name="disposeManagedResources">Whether to dispose managed resources.</param>
        protected virtual void Dispose(bool disposeManagedResources)
        {
            if (_disposed) return;

            if (disposeManagedResources)
            {
                _eventHorizons.Dispose();
            }

            _disposed = true;
        }
    }
}

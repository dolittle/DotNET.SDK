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
using Dolittle.SDK.Resources;
using Dolittle.SDK.Services;
using Dolittle.SDK.Tenancy.Client;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK
{
    /// <summary>
    /// Represents the client for working with the Dolittle Runtime.
    /// </summary>
    public class DolittleClient : IDisposable, IDolittleClient
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
        /// Initializes a new instance of the <see cref="DolittleClient" /> class.
        /// </summary>
        /// <param name="eventTypes">The <see cref="EventTypes" />.</param>
        /// <param name="eventStoreBuilder">The <see cref="IEventStoreBuilder" />.</param>
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
        /// <param name="projectionStoreBuilder">The <see cref="IProjectionStoreBuilder" />.</param>
        /// <param name="embeddings">The <see cref="IEmbeddings" />.</param>
        /// <param name="aggregateRoots">The <see cref="IAggregateRoots"/>.</param>
        /// <param name="tenants">The <see cref="ITenants"/>.</param>
        /// <param name="resources">The <see cref="IResourcesBuilder"/>.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        public DolittleClient(
            IEventTypes eventTypes,
            IEventStoreBuilder eventStoreBuilder,
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
            IProjectionStoreBuilder projectionStoreBuilder,
            IEmbeddings embeddings,
            IAggregateRoots aggregateRoots,
            ITenants tenants,
            IResourcesBuilder resources,
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
            Resources = resources;
            _loggerFactory = loggerFactory;
            _cancellation = cancellationToken;
            _container = new DefaultContainer();
        }

        /// <inheritdoc />
        public IEventTypes EventTypes { get; }

        /// <inheritdoc />
        public IEventStoreBuilder EventStore { get; }

        /// <inheritdoc />
        public IProjectionStoreBuilder Projections { get; }

        /// <inheritdoc />
        public IEmbeddings Embeddings { get; }

        /// <inheritdoc />
        public IEventHorizons EventHorizons => _eventHorizons;

        /// <inheritdoc />
        public ITenants Tenants { get; }

        /// <inheritdoc />
        public IResourcesBuilder Resources { get; }

        /// <summary>
        /// Create a client builder for a Microservice.
        /// </summary>
        /// <param name="microserviceId">The unique identifier for the microservice.</param>
        /// <returns>The <see cref="DolittleClientBuilder"/> to build the <see cref="DolittleClient"/> from.</returns>
        public static DolittleClientBuilder ForMicroservice(MicroserviceId microserviceId)
            => new DolittleClientBuilder(microserviceId);

        /// <inheritdoc />
        public IDolittleClient WithContainer(IContainer container)
        {
            _container = container;
            return this;
        }

        /// <inheritdoc/>
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

        /// <inheritdoc />
        public IAggregateRootOperations<TAggregateRoot> AggregateOf<TAggregateRoot>(Func<IEventStoreBuilder, IEventStore> buildEventStore)
            where TAggregateRoot : AggregateRoot
            => new AggregateOf<TAggregateRoot>(buildEventStore(EventStore), EventTypes, _aggregateRoots, _loggerFactory)
                    .Create();

        /// <inheritdoc />
        public IAggregateRootOperations<TAggregateRoot> AggregateOf<TAggregateRoot>(EventSourceId eventSource, Func<IEventStoreBuilder, IEventStore> buildEventStore)
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

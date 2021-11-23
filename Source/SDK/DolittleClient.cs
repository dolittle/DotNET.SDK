// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Aggregates.Builders;
using Dolittle.SDK.Aggregates.Internal;
using Dolittle.SDK.DependencyInversion;
using Dolittle.SDK.Embeddings;
using Dolittle.SDK.Embeddings.Builder;
using Dolittle.SDK.Embeddings.Store;
using Dolittle.SDK.EventHorizon;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Builders;
using Dolittle.SDK.Events.Filters;
using Dolittle.SDK.Events.Handling.Builder;
using Dolittle.SDK.Events.Processing;
using Dolittle.SDK.Events.Store;
using Dolittle.SDK.Events.Store.Builders;
using Dolittle.SDK.Events.Store.Converters;
using Dolittle.SDK.Projections.Builder;
using Dolittle.SDK.Projections.Store;
using Dolittle.SDK.Projections.Store.Builders;
using Dolittle.SDK.Projections.Store.Converters;
using Dolittle.SDK.Resources;
using Dolittle.SDK.Services;
using Dolittle.SDK.Tenancy;
using Dolittle.SDK.Tenancy.Client.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK
{
    /// <summary>
    /// Represents the client for working with the Dolittle Runtime.
    /// </summary>
    public class DolittleClient : IDisposable, IDolittleClient
    {
        readonly EventTypesBuilder _eventTypesBuilder;
        readonly IProjectionReadModelTypeAssociations _projectionAssociations;
        readonly IEmbeddingReadModelTypeAssociations _embeddingAssociations;
        readonly AggregateRootsBuilder _aggregateRootsBuilder;
        readonly SubscriptionsBuilder _eventHorizonsBuilder;
        readonly EventSubscriptionRetryPolicy _eventHorizonRetryPolicy;
        readonly EventHandlersBuilder _eventHandlersBuilder;
        readonly ICoordinateProcessing _processingCoordinator = new ProcessingCoordinator();
        readonly EventFiltersBuilder _filtersBuilder;
        readonly IConvertProjectionsToSDK _projectionConverter = new ProjectionsToSDKConverter();
        readonly IResolveCallContext _callContextResolver = new CallContextResolver();
        readonly ProjectionsBuilder _projectionsBuilder;
        readonly EmbeddingsBuilder _embeddingsBuilder;

        IConvertEventsToProtobuf _eventsToProtobufConverter;
        EventHorizons _eventHorizons;
        IEventProcessors _eventProcessors;
        IEventProcessingConverter _eventProcessingConverter;
        IAggregateRoots _aggregateRoots;
        ILoggerFactory _loggerFactory;
        CancellationToken _cancellation;
        bool _disposed;
        bool _connected;
        bool _ready;
        IServiceProvider _serviceProvider;
        Action<TenantId, IServiceCollection> _configureTenantContainers;
        IProjectionStoreBuilder _projections;
        IEmbeddings _embeddings;
        IContainer _services;

        /// <summary>
        /// Initializes a new instance of the <see cref="DolittleClient"/> class.
        /// </summary>
        /// <param name="eventTypesBuilder">The <see cref="EventTypesBuilder"/>.</param>
        /// <param name="projectionAssociations">The <see cref="IProjectionReadModelTypeAssociations"/>.</param>
        /// <param name="embeddingAssociations">The <see cref="IEmbeddingReadModelTypeAssociations"/>.</param>
        /// <param name="eventHandlersBuilder">The <see cref="EventHandlerBuilder"/>.</param>
        /// <param name="aggregateRootsBuilder">The <see cref="AggregateRootsBuilder"/>.</param>
        /// <param name="projectionsBuilder">The <see cref="ProjectionsBuilder"/>.</param>
        /// <param name="embeddingsBuilder">The <see cref="EmbeddingsBuilder"/>.</param>
        /// <param name="eventFiltersBuilder">The <see cref="EventFiltersBuilder"/>.</param>
        /// <param name="eventHorizonsBuilder">The <see cref="SubscriptionsBuilder"/>.</param>
        /// <param name="eventHorizonRetryPolicy">The <see cref="EventSubscriptionRetryPolicy"/>.</param>
        public DolittleClient(
            EventTypesBuilder eventTypesBuilder,
            IProjectionReadModelTypeAssociations projectionAssociations,
            IEmbeddingReadModelTypeAssociations embeddingAssociations,
            EventHandlersBuilder eventHandlersBuilder,
            AggregateRootsBuilder aggregateRootsBuilder,
            ProjectionsBuilder projectionsBuilder,
            EmbeddingsBuilder embeddingsBuilder,
            EventFiltersBuilder eventFiltersBuilder,
            SubscriptionsBuilder eventHorizonsBuilder,
            EventSubscriptionRetryPolicy eventHorizonRetryPolicy)
        {
            _eventTypesBuilder = eventTypesBuilder;
            _projectionAssociations = projectionAssociations;
            _embeddingAssociations = embeddingAssociations;
            _eventHandlersBuilder = eventHandlersBuilder;
            _aggregateRootsBuilder = aggregateRootsBuilder;
            _projectionsBuilder = projectionsBuilder;
            _embeddingsBuilder = embeddingsBuilder;
            _filtersBuilder = eventFiltersBuilder;
            _eventHorizonsBuilder = eventHorizonsBuilder;
            _eventHorizonRetryPolicy = eventHorizonRetryPolicy;
        }

        /// <inheritdoc />
        public IEventTypes EventTypes { get; private set; }

        /// <inheritdoc />
        public IEventStoreBuilder EventStore { get; private set; }

        /// <inheritdoc />
        public IEventHorizons EventHorizons => _eventHorizons;

        /// <inheritdoc />
        public IEnumerable<Tenant> Tenants { get; private set; }

        /// <inheritdoc />
        public IResourcesBuilder Resources { get; private set; }

        /// <inheritdoc />
        public IProjectionStoreBuilder Projections
        {
            get => GetOrThrowIfNotReady(_projections);
            private set => _projections = value;
        }

        /// <inheritdoc />
        public IEmbeddings Embeddings
        {
            get => GetOrThrowIfNotReady(_embeddings);
            private set => _embeddings = value;
        }

        TRequiresStartService GetOrThrowIfNotReady<TRequiresStartService>(TRequiresStartService service)
        {
            if (!_ready)
            {
                throw new DolittleClientNotStarted();
            }

            return service;
        }

        /// <inheritdoc />
        public IContainer Services
        {
            get => _services ?? throw new DolittleClientNotStarted();
            private set => _services = value;
        }

        /// <summary>
        /// Create a client builder for a Microservice.
        /// </summary>
        /// <returns>The <see cref="DolittleClientBuilder"/> to build the <see cref="DolittleClient"/> from.</returns>
        public static DolittleClientBuilder Setup()
            => new DolittleClientBuilder();

        /// <summary>
        /// Configures the root <see cref="IServiceProvider"/>.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/>.</param>
        /// <returns>The client for continuation.</returns>
        public DolittleClient WithServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            return this;
        }

        /// <summary>
        /// Configures a <see cref="Action{T}"/> callback for configuring the tenant specific IoC containers.
        /// </summary>
        /// <param name="configureTenantContainers">The <see cref="Action{T}"/> callback for configuring the tenant specific IoC containers</param>
        /// <returns>The client for continuation.</returns>
        public DolittleClient WithTenantServices(Action<TenantId, IServiceCollection> configureTenantContainers) // Maybe hook in with TContainerBuilder instead? That way we can allow for using the Autofac ContainerBuilder for instance
        {
            _configureTenantContainers = configureTenantContainers;
            return this;
        }

        /// <inheritdoc/>
        public Task Start()
        {
            if (!_connected)
            {
                throw new CannotStartUnconnectedDolittleClient();
            }

            ConfigureContainer();

            // Should use container when creating aggregate roots? We at least though of that at some point
            _aggregateRoots = new AggregateRoots(_loggerFactory.CreateLogger<AggregateRoots>());
            _eventHandlersBuilder.BuildAndRegister(
                _eventProcessors,
                EventTypes,
                _eventProcessingConverter,
                Services,
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

            _ready = true;
            return _processingCoordinator.Completion;
        }

        /// <summary>
        /// Connects the <see cref="IDolittleClient"/>.
        /// </summary>
        /// <param name="configureClient">The optional <see cref="Action{T}"/> callback for configuring the <see cref="DolittleClientConfiguration"/>.</param>
        /// <param name="configureConnection">The optional <see cref="Action{T}"/> callback for configuring the <see cref="DolittleClientConnectionArguments"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task Connect(
            Action<DolittleClientConfigurationBuilder> configureClient = default,
            Action<DolittleClientConnectionArguments> configureConnection = default)
        {
            var connectionConfig = new DolittleClientConnectionArguments();
            configureConnection?.Invoke(connectionConfig);
            return Connect(configureClient, connectionConfig);
        }

        /// <summary>
        /// Connects the <see cref="IDolittleClient"/>.
        /// </summary>
        /// <param name="configureClient">The optional <see cref="Action{T}"/> callback for configuring the <see cref="DolittleClientConfiguration"/>.</param>
        /// <param name="executionContextResolver">The optional <see cref="ICanResolveExecutionContextForDolittleClient"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task Connect(
            Action<DolittleClientConfigurationBuilder> configureClient = default,
            ICanResolveExecutionContextForDolittleClient executionContextResolver = default)
        {
            executionContextResolver ??= new DolittleClientConnectionArguments();
            var clientConfigBuilder = DolittleClientConfiguration.Builder;
            configureClient?.Invoke(clientConfigBuilder);
            var clientConfig = clientConfigBuilder.Build();
            var executionContext = await executionContextResolver.Resolve().ConfigureAwait(false);

            await PerformHandshake(executionContext).ConfigureAwait(false);
            await RegisterAndCreateDependencies(clientConfig, executionContext).ConfigureAwait(false);
            _connected = true;
        }

        /// <inheritdoc />
        public IAggregateRootOperations<TAggregateRoot> AggregateOf<TAggregateRoot>(Func<IEventStoreBuilder, IEventStore> buildEventStore)
            where TAggregateRoot : AggregateRoot
            => new AggregateOf<TAggregateRoot>(buildEventStore(EventStore), EventTypes, GetOrThrowIfNotReady(_aggregateRoots), _loggerFactory)
                    .Create();

        /// <inheritdoc />
        public IAggregateRootOperations<TAggregateRoot> AggregateOf<TAggregateRoot>(EventSourceId eventSource, Func<IEventStoreBuilder, IEventStore> buildEventStore)
            where TAggregateRoot : AggregateRoot
            => new AggregateOf<TAggregateRoot>(buildEventStore(EventStore), EventTypes, GetOrThrowIfNotReady(_aggregateRoots), _loggerFactory)
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
                _eventHorizons?.Dispose();
            }

            _disposed = true;
        }

        async Task RegisterAndCreateDependencies(DolittleClientConfiguration clientConfig, ExecutionContext executionContext)
        {
            _loggerFactory = clientConfig.LoggerFactory;
            _cancellation = clientConfig.Cancellation;
            var methodCaller = new MethodCaller(clientConfig.RuntimeHost, clientConfig.RuntimePort);
            EventTypes = new EventTypes(_loggerFactory.CreateLogger<EventTypes>());
            _eventTypesBuilder.AddAssociationsInto(EventTypes);
            await _eventTypesBuilder.BuildAndRegister(
                new Events.Internal.EventTypesClient(
                    methodCaller,
                    executionContext,
                    _loggerFactory.CreateLogger<Events.Internal.EventTypesClient>()),
                _cancellation).ConfigureAwait(false);
            await _aggregateRootsBuilder.BuildAndRegister(
                new AggregateRootsClient(
                    methodCaller,
                    executionContext,
                    _loggerFactory.CreateLogger<AggregateRoots>()),
                _cancellation).ConfigureAwait(false);
            var reverseCallClientsCreator = new ReverseCallClientCreator(
                clientConfig.PingInterval,
                methodCaller,
                executionContext,
                _loggerFactory);
            var serializer = new EventContentSerializer(EventTypes, clientConfig.EventSerializerProvider);
            _eventsToProtobufConverter = new EventToProtobufConverter(serializer);
            var eventToSDKConverter = new EventToSDKConverter(serializer);
            var aggregateEventToProtobufConverter = new AggregateEventToProtobufConverter(serializer);
            var aggregateEventToSDKConverter = new AggregateEventToSDKConverter(serializer);
            _eventProcessingConverter = new EventProcessingConverter(eventToSDKConverter);
            _eventProcessors = new EventProcessors(
                reverseCallClientsCreator,
                _processingCoordinator,
                _loggerFactory.CreateLogger<EventProcessors>());
            EventStore = new EventStoreBuilder(
                methodCaller,
                _eventsToProtobufConverter,
                eventToSDKConverter,
                aggregateEventToProtobufConverter,
                aggregateEventToSDKConverter,
                executionContext,
                _callContextResolver,
                EventTypes,
                _loggerFactory);

            _eventHorizons = new EventHorizons(
                methodCaller,
                executionContext,
                _eventHorizonRetryPolicy,
                _loggerFactory.CreateLogger<EventHorizons>());
            _eventHorizonsBuilder.BuildAndSubscribe(_eventHorizons, _cancellation);

            Projections = new ProjectionStoreBuilder(
                methodCaller,
                executionContext,
                _callContextResolver,
                _projectionAssociations,
                _projectionConverter,
                _loggerFactory);
            Embeddings = new Embeddings.Embeddings(
                methodCaller,
                _callContextResolver,
                _embeddingAssociations,
                _projectionConverter,
                executionContext,
                _loggerFactory);

            var tenants = new TenantsClient(methodCaller, executionContext, _loggerFactory.CreateLogger<TenantsClient>());
            Resources = new ResourcesBuilder(methodCaller, executionContext, _loggerFactory);
            Tenants = await tenants.GetAll(_cancellation).ConfigureAwait(false);
        }

        void ConfigureContainer()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.UseRootProvider(_serviceProvider);
            foreach (var tenant in Tenants)
            {
                containerBuilder.AddTenant(tenant.Id);
            }

            containerBuilder.AddTenantServices(_configureTenantContainers);
            Services = containerBuilder.Build();
        }

        Task PerformHandshake(ExecutionContext executionContext)
        {
            return Task.CompletedTask;
        }
    }
}

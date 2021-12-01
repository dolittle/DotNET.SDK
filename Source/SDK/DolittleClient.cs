// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
using Dolittle.SDK.Events.Store.Builders;
using Dolittle.SDK.Events.Store.Converters;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Microservices;
using Dolittle.SDK.Projections.Builder;
using Dolittle.SDK.Projections.Store;
using Dolittle.SDK.Projections.Store.Builders;
using Dolittle.SDK.Projections.Store.Converters;
using Dolittle.SDK.Resources;
using Dolittle.SDK.Security;
using Dolittle.SDK.Services;
using Dolittle.SDK.Tenancy;
using Dolittle.SDK.Tenancy.Client.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;
using Environment = Dolittle.SDK.Microservices.Environment;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;
using Version = Dolittle.SDK.Microservices.Version;

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
        IProjectionStoreBuilder _projections;
        IEmbeddings _embeddings;
        ITenantScopedProviders _services;

        bool _disposed;
        IEnumerable<Tenant> _tenants;
        IResourcesBuilder _resources;
        IEventStoreBuilder _eventStore;
        IEventTypes _eventTypes;
        IAggregatesBuilder _aggregates;
        CancellationTokenSource _eventProcessorCancellationTokenSource;

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
        public bool Connected { get; private set; }

        /// <inheritdoc />
        public IEventTypes EventTypes
        {
            get => GetOrThrowIfNotConnected(_eventTypes);
            private set => _eventTypes = value;
        }

        /// <inheritdoc />
        public IEventStoreBuilder EventStore
        {
            get => GetOrThrowIfNotConnected(_eventStore);
            private set => _eventStore = value;
        }

        /// <inheritdoc />
        public IAggregatesBuilder Aggregates
        {
            get => GetOrThrowIfNotConnected(_aggregates);
            private set => _aggregates = value;
        }

        /// <inheritdoc />
        public IEventHorizons EventHorizons
        {
            get => GetOrThrowIfNotConnected(_eventHorizons);
            private set => _eventHorizons = value as EventHorizons;
        }

        /// <inheritdoc />
        public IEnumerable<Tenant> Tenants
        {
            get => GetOrThrowIfNotConnected(_tenants);
            private set => _tenants = value;
        }

        /// <inheritdoc />
        public IResourcesBuilder Resources
        {
            get => GetOrThrowIfNotConnected(_resources);
            private set => _resources = value;
        }

        /// <inheritdoc />
        public IProjectionStoreBuilder Projections
        {
            get => GetOrThrowIfNotConnected(_projections);
            private set => _projections = value;
        }

        /// <inheritdoc />
        public IEmbeddings Embeddings
        {
            get => GetOrThrowIfNotConnected(_embeddings);
            private set => _embeddings = value;
        }

        /// <inheritdoc />
        public ITenantScopedProviders Services
        {
            get => GetOrThrowIfNotConnected(_services);
            private set => _services = value;
        }

        /// <summary>
        /// Create a client builder for a Microservice.
        /// </summary>
        /// <param name="setup">The optional <see cref="SetupDolittleClient"/> callback.</param>
        /// <returns>The <see cref="DolittleClientBuilder"/> to build the <see cref="DolittleClient"/> from.</returns>
        public static IDolittleClient Setup(SetupDolittleClient setup = default)
        {
            var builder = new DolittleClientBuilder();
            setup?.Invoke(builder);
            return builder.Build();
        }

        /// <inheritdoc />
        public Task<IDolittleClient> Connect(ConfigureDolittleClient configureClient, CancellationToken cancellationToken = default)
        {
            var configuration = new DolittleClientConfiguration();
            configureClient?.Invoke(configuration);
            return Connect(configuration, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IDolittleClient> Connect(CancellationToken cancellationToken = default)
            => Connect(new DolittleClientConfiguration(), cancellationToken);

        /// <inheritdoc />
        public async Task<IDolittleClient> Connect(DolittleClientConfiguration configuration, CancellationToken cancellationToken = default)
        {
            if (Connected)
            {
                throw new CannotConnectDolittleClientMultipleTimes();
            }

            var executionContext = await PerformHandshake(configuration.RuntimeHost, configuration.RuntimePort, cancellationToken).ConfigureAwait(false);
            var tenantScopedProvidersBuilder = new TenantScopedProvidersBuilder();
            await RegisterAndCreateDependencies(
                configuration.RuntimeHost,
                configuration.RuntimePort,
                configuration.PingInterval,
                configuration.EventSerializerProvider,
                configuration.LoggerFactory,
                executionContext,
                tenantScopedProvidersBuilder,
                cancellationToken).ConfigureAwait(false);
            StartEventProcessors(tenantScopedProvidersBuilder, configuration.LoggerFactory, cancellationToken);
            Connected = true;

            // It's currently important that the container gets built after registering and starting the event processors, because they add tenant scoped services themselves.
            ConfigureContainer(tenantScopedProvidersBuilder, configuration);
            return this;
        }

        /// <inheritdoc />
        public Task Disconnect(CancellationToken cancellationToken = default)
        {
            _eventProcessorCancellationTokenSource.Cancel();
            return _processingCoordinator.Completion;
        }

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
                _eventProcessorCancellationTokenSource?.Dispose();
                _eventHorizons?.Dispose();
            }

            _disposed = true;
        }

        async Task RegisterAndCreateDependencies(
            string runtimeHost,
            ushort runtimePort,
            TimeSpan pingInterval,
            Func<JsonSerializerSettings> eventSerializerProvider,
            ILoggerFactory loggerFactory,
            ExecutionContext executionContext,
            TenantScopedProvidersBuilder tenantScopedProvidersBuilder,
            CancellationToken cancellation)
        {
            var aggregateRoots = new AggregateRoots(loggerFactory.CreateLogger<AggregateRoots>());
            var methodCaller = new MethodCaller(runtimeHost, runtimePort);
            EventTypes = new EventTypes(loggerFactory.CreateLogger<EventTypes>());
            _eventTypesBuilder.AddAssociationsInto(_eventTypes);
            await _eventTypesBuilder.BuildAndRegister(
                new Events.Internal.EventTypesClient(
                    methodCaller,
                    executionContext,
                    loggerFactory.CreateLogger<Events.Internal.EventTypesClient>()),
                cancellation).ConfigureAwait(false);
            var reverseCallClientsCreator = new ReverseCallClientCreator(
                pingInterval,
                methodCaller,
                executionContext,
                loggerFactory);
            var serializer = new EventContentSerializer(_eventTypes, eventSerializerProvider);
            _eventsToProtobufConverter = new EventToProtobufConverter(serializer);
            var eventToSDKConverter = new EventToSDKConverter(serializer);
            var aggregateEventToProtobufConverter = new AggregateEventToProtobufConverter(serializer);
            var aggregateEventToSDKConverter = new AggregateEventToSDKConverter(serializer);
            _eventProcessingConverter = new EventProcessingConverter(eventToSDKConverter);
            _eventProcessors = new EventProcessors(
                reverseCallClientsCreator,
                _processingCoordinator,
                loggerFactory.CreateLogger<EventProcessors>());
            EventStore = new EventStoreBuilder(
                methodCaller,
                _eventsToProtobufConverter,
                eventToSDKConverter,
                aggregateEventToProtobufConverter,
                aggregateEventToSDKConverter,
                executionContext,
                _callContextResolver,
                _eventTypes,
                loggerFactory);
            Aggregates = new AggregatesBuilder(
                _eventStore,
                _eventTypes,
                aggregateRoots,
                loggerFactory);

            await _aggregateRootsBuilder.BuildAndRegister(
                new AggregateRootsClient(
                    methodCaller,
                    executionContext,
                    loggerFactory.CreateLogger<AggregateRoots>()),
                tenantScopedProvidersBuilder,
                _aggregates,
                cancellation).ConfigureAwait(false);
            EventHorizons = new EventHorizons(
                methodCaller,
                executionContext,
                _eventHorizonRetryPolicy,
                loggerFactory.CreateLogger<EventHorizons>());
            _eventHorizonsBuilder.BuildAndSubscribe(_eventHorizons, cancellation);

            Projections = new ProjectionStoreBuilder(
                methodCaller,
                executionContext,
                _callContextResolver,
                _projectionAssociations,
                _projectionConverter,
                loggerFactory);
            Embeddings = new Embeddings.Embeddings(
                methodCaller,
                _callContextResolver,
                _embeddingAssociations,
                _projectionConverter,
                executionContext,
                loggerFactory);

            var tenants = new TenantsClient(methodCaller, executionContext, loggerFactory.CreateLogger<TenantsClient>());
            Resources = new ResourcesBuilder(methodCaller, executionContext, loggerFactory);
            Tenants = await tenants.GetAll(cancellation).ConfigureAwait(false);
        }

        void StartEventProcessors(TenantScopedProvidersBuilder tenantScopedProvidersBuilder, ILoggerFactory loggerFactory, CancellationToken cancellationToken)
        {
            _eventProcessorCancellationTokenSource = new CancellationTokenSource();
            _eventHandlersBuilder.BuildAndRegister(
                _eventProcessors,
                _eventTypes,
                _eventProcessingConverter,
                tenantScopedProvidersBuilder,
                () => Services,
                loggerFactory,
                cancellationToken,
                GetStopProcessingToken());
            _filtersBuilder.BuildAndRegister(
                _eventProcessors,
                _eventProcessingConverter,
                loggerFactory,
                cancellationToken,
                GetStopProcessingToken());
            _projectionsBuilder.BuildAndRegister(
                _eventProcessors,
                _eventTypes,
                _eventProcessingConverter,
                _projectionConverter,
                loggerFactory,
                cancellationToken,
                GetStopProcessingToken());
            _embeddingsBuilder.BuildAndRegister(
                _eventProcessors,
                _eventTypes,
                _eventsToProtobufConverter,
                _projectionConverter,
                loggerFactory,
                cancellationToken,
                GetStopProcessingToken());
        }

        TRequiresStartService GetOrThrowIfNotConnected<TRequiresStartService>(TRequiresStartService service)
        {
            if (!Connected)
            {
                throw new CannotUseUnconnectedDolittleClient();
            }

            return service;
        }

        void ConfigureContainer(TenantScopedProvidersBuilder tenantScopedProvidersBuilder, DolittleClientConfiguration config)
        {
            Services = tenantScopedProvidersBuilder
                .WithRoot(config.ServiceProvider)
                .WithTenants(_tenants.Select(_ => _.Id))
                .AddTenantServices(AddBuilderServices)
                .AddTenantServices(config.ConfigureTenantServices)
                .Build();
        }

        void AddBuilderServices(TenantId tenant, IServiceCollection services)
            => services
                .AddScoped(_ => EventStore.ForTenant(tenant))
                .AddScoped(_ => Aggregates.ForTenant(tenant))
                .AddScoped(_ => Projections.ForTenant(tenant))
                .AddScoped(_ => Embeddings.ForTenant(tenant))
                .AddScoped(_ => Resources.ForTenant(tenant));

        Task<ExecutionContext> PerformHandshake(string runtimeHost, ushort runtimePort, CancellationToken cancellationToken)
        {
            // TODO: Connect and get from platform.
            return Task.FromResult(new ExecutionContext(
                MicroserviceId.NotSet,
                TenantId.Development,
                Version.NotSet,
                Environment.Undetermined,
                CorrelationId.System,
                Claims.Empty,
                CultureInfo.InvariantCulture));
        }

        CancellationToken GetStopProcessingToken() => _eventProcessorCancellationTokenSource.Token;
    }
}

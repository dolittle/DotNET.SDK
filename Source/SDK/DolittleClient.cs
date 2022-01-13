// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Aggregates.Builders;
using Dolittle.SDK.Aggregates.Internal;
using Dolittle.SDK.Builders;
using Dolittle.SDK.DependencyInversion;
using Dolittle.SDK.Embeddings;
using Dolittle.SDK.Embeddings.Builder;
using Dolittle.SDK.Embeddings.Store;
using Dolittle.SDK.EventHorizon;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Builders;
using Dolittle.SDK.Events.Filters;
using Dolittle.SDK.Events.Filters.Builders;
using Dolittle.SDK.Events.Handling.Builder;
using Dolittle.SDK.Events.Processing;
using Dolittle.SDK.Events.Store.Builders;
using Dolittle.SDK.Events.Store.Converters;
using Dolittle.SDK.Handshake;
using Dolittle.SDK.Handshake.Internal;
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
using Newtonsoft.Json;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK;

/// <summary>
/// Represents the client for working with the Dolittle Runtime.
/// </summary>
public class DolittleClient : IDisposable, IDolittleClient
{
    readonly ICoordinateProcessing _processingCoordinator = new ProcessingCoordinator();
    readonly IConvertProjectionsToSDK _projectionConverter = new ProjectionsToSDKConverter();
    readonly IResolveCallContext _callContextResolver = new CallContextResolver();
    readonly IUnregisteredEventTypes _unregisteredEventTypes;
    readonly IUnregisteredAggregateRoots _unregisteredAggregateRoots;
    readonly IUnregisteredEventFilters _unregisteredEventFilters;
    readonly EventHandlersBuilder _eventHandlersBuilder;
    readonly IUnregisteredProjections _unregisteredProjections;
    readonly IUnregisteredEmbeddings _unregisteredEmbeddings;
    readonly SubscriptionsBuilder _eventHorizonsBuilder;
    readonly EventSubscriptionRetryPolicy _eventHorizonRetryPolicy;

    IConvertEventsToProtobuf _eventsToProtobufConverter;
    EventHorizons _eventHorizons;
    IEventProcessors _eventProcessors;
    IEventProcessingConverter _eventProcessingConverter;
    IProjectionStoreBuilder _projectionStoreBuilder;
    IEventTypes _eventTypes;
    IEmbeddings _embeddingStoreBuilder;
    ITenantScopedProviders _services;

    bool _disposed;
    IEnumerable<Tenant> _tenants;
    IResourcesBuilder _resources;
    IEventStoreBuilder _eventStore;
    IAggregatesBuilder _aggregates;
    CancellationTokenSource _eventProcessorCancellationTokenSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="DolittleClient"/> class.
    /// </summary>
    /// <param name="unregisteredEventTypes">The <see cref="IUnregisteredEventTypes"/>.</param>
    /// <param name="unregisteredAggregateRoots">The <see cref="IUnregisteredAggregateRoots"/>.</param>
    /// <param name="unregisteredEventFilters">The <see cref="IUnregisteredEventFilters"/>.</param>
    /// <param name="eventHandlersBuilder">The <see cref="EventHandlerBuilder"/>.</param>
    /// <param name="unregisteredProjections">The <see cref="IUnregisteredProjections"/>.</param>
    /// <param name="unregisteredEmbeddings">The <see cref="IUnregisteredEmbeddings"/>.</param>
    /// <param name="eventHorizonsBuilder">The <see cref="SubscriptionsBuilder"/>.</param>
    /// <param name="eventHorizonRetryPolicy">The <see cref="EventSubscriptionRetryPolicy"/>.</param>
    public DolittleClient(
        IUnregisteredEventTypes unregisteredEventTypes,
        IUnregisteredAggregateRoots unregisteredAggregateRoots,
        IUnregisteredEventFilters unregisteredEventFilters,
        EventHandlersBuilder eventHandlersBuilder,
        IUnregisteredProjections unregisteredProjections,
        IUnregisteredEmbeddings unregisteredEmbeddings,
        SubscriptionsBuilder eventHorizonsBuilder,
        EventSubscriptionRetryPolicy eventHorizonRetryPolicy)
    {
        _unregisteredEventTypes = unregisteredEventTypes;
        _unregisteredAggregateRoots = unregisteredAggregateRoots;
        _unregisteredEventFilters = unregisteredEventFilters;
        _eventHandlersBuilder = eventHandlersBuilder;
        _unregisteredProjections = unregisteredProjections;
        _unregisteredEmbeddings = unregisteredEmbeddings;
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
        get => GetOrThrowIfNotConnected(_projectionStoreBuilder);
        private set => _projectionStoreBuilder = value;
    }

    /// <inheritdoc />
    public IEmbeddings Embeddings
    {
        get => GetOrThrowIfNotConnected(_embeddingStoreBuilder);
        private set => _embeddingStoreBuilder = value;
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
    /// <returns>The built <see cref="IDolittleClient"/>.</returns>
    public static IDolittleClient Setup(SetupDolittleClient setup = default)
    {
        var builder = new SetupBuilder();
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

        var methodCaller = new MethodCaller(configuration.RuntimeHost, configuration.RuntimePort);
        var loggerFactory = configuration.LoggerFactory;
        var runtimeConnector = new DolittleRuntimeConnector(
            configuration.RuntimeHost,
            configuration.RuntimePort,
            new HandshakeClient(methodCaller, loggerFactory.CreateLogger<HandshakeClient>()),
            new TenantsClient(methodCaller, loggerFactory.CreateLogger<TenantsClient>()),
            loggerFactory.CreateLogger<DolittleRuntimeConnector>());
        (var executionContext, Tenants) = await runtimeConnector.ConnectForever(cancellationToken).ConfigureAwait(false);
        var tenantScopedProvidersBuilder = new TenantScopedProvidersBuilder();
        await RegisterAndCreateDependencies(
            methodCaller,
            configuration.PingInterval,
            configuration.EventSerializerProvider,
            loggerFactory,
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
        return Task.WhenAny(_processingCoordinator.Completion, Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken));
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
        if (_disposed)
        {
            return;
        }

        if (disposeManagedResources)
        {
            _eventProcessorCancellationTokenSource?.Dispose();
            _eventHorizons?.Dispose();
        }

        _disposed = true;
    }

    async Task RegisterAndCreateDependencies(
        IPerformMethodCalls methodCaller,
        TimeSpan pingInterval,
        Func<JsonSerializerSettings> eventSerializerProvider,
        ILoggerFactory loggerFactory,
        ExecutionContext executionContext,
        TenantScopedProvidersBuilder tenantScopedProvidersBuilder,
        CancellationToken cancellation)
    {
        var aggregateRoots = new AggregateRoots(loggerFactory.CreateLogger<AggregateRoots>());
        EventTypes = new EventTypes(loggerFactory.CreateLogger<EventTypes>());
        _eventTypesBuilder.AddAssociationsInto(_unregisteredEventTypes);
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
        var serializer = new EventContentSerializer(_unregisteredEventTypes, eventSerializerProvider);
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
            _unregisteredEventTypes,
            loggerFactory);
        Aggregates = new AggregatesBuilder(
            _eventStore,
            _unregisteredEventTypes,
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
        Resources = new ResourcesBuilder(methodCaller, executionContext, loggerFactory);
    }

    void StartEventProcessors(TenantScopedProvidersBuilder tenantScopedProvidersBuilder, ILoggerFactory loggerFactory, CancellationToken cancellationToken)
    {
        _eventProcessorCancellationTokenSource = new CancellationTokenSource();
        _eventHandlersBuilder.BuildAndRegister(
            _eventProcessors,
            _unregisteredEventTypes,
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
            _unregisteredEventTypes,
            _eventProcessingConverter,
            _projectionConverter,
            loggerFactory,
            cancellationToken,
            GetStopProcessingToken());
        _embeddingsBuilder.BuildAndRegister(
            _eventProcessors,
            _unregisteredEventTypes,
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

    CancellationToken GetStopProcessingToken() => _eventProcessorCancellationTokenSource.Token;
}

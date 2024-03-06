// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
#if NET5_0_OR_GREATER
using System.Net.Http;
#endif
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Aggregates.Builders;
using Dolittle.SDK.Aggregates.Internal;
using Dolittle.SDK.Builders;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.DependencyInversion;
using Dolittle.SDK.EventHorizon;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Builders;
using Dolittle.SDK.Events.Filters.Builders;
using Dolittle.SDK.Events.Handling.Builder;
using Dolittle.SDK.Events.Processing;
using Dolittle.SDK.Events.Store.Builders;
using Dolittle.SDK.Events.Store.Converters;
using Dolittle.SDK.Handshake;
using Dolittle.SDK.Handshake.Internal;
using Dolittle.SDK.Projections.Builder;
using Dolittle.SDK.Projections.Store.Builders;
using Dolittle.SDK.Resources;
using Dolittle.SDK.Resources.Internal;
using Dolittle.SDK.Services;
using Dolittle.SDK.Tenancy;
using Dolittle.SDK.Tenancy.Client.Internal;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK;

/// <summary>
/// Represents the client for working with the Dolittle Runtime.
/// </summary>
public class DolittleClient : IDisposable, IDolittleClient
{
    readonly ICoordinateProcessing _processingCoordinator = new ProcessingCoordinator();
    readonly IResolveCallContext _callContextResolver = new CallContextResolver();

    readonly IClientBuildResults _buildResults;
    readonly IUnregisteredEventTypes _unregisteredEventTypes;
    readonly IUnregisteredAggregateRoots _unregisteredAggregateRoots;
    readonly IUnregisteredEventFilters _unregisteredEventFilters;
    readonly IUnregisteredEventHandlers _unregisteredEventHandlers;
    readonly IUnregisteredProjections _unregisteredProjections;
    readonly SubscriptionsBuilder _eventHorizonsBuilder;
    readonly EventSubscriptionRetryPolicy _eventHorizonRetryPolicy;
    readonly SemaphoreSlim _connectLock = new(1, 1);
    readonly TaskCompletionSource<bool> _connectedCompletionSource = new(TaskCreationOptions.RunContinuationsAsynchronously);

    IConvertEventsToProtobuf _eventsToProtobufConverter;
    EventHorizons _eventHorizons;
    IProjectionStoreBuilder _projectionStoreBuilder;
    ITenantScopedProviders _services;

    bool _disposed;
    IEnumerable<Tenant> _tenants;
    IResourcesBuilder _resources;
    IEventStoreBuilder _eventStore;
    IAggregatesBuilder _aggregates;
    CancellationTokenSource _clientCancellationTokenSource;
    EventToSDKConverter _eventToSDKConverter;
    GrpcChannel _grpcChannel;

    /// <summary>
    /// Initializes a new instance of the <see cref="DolittleClient"/> class.
    /// </summary>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    /// <param name="unregisteredEventTypes">The <see cref="IUnregisteredEventTypes"/>.</param>
    /// <param name="unregisteredAggregateRoots">The <see cref="IUnregisteredAggregateRoots"/>.</param>
    /// <param name="unregisteredEventFilters">The <see cref="IUnregisteredEventFilters"/>.</param>
    /// <param name="unregisteredEventHandlers">The <see cref="EventHandlerBuilder"/>.</param>
    /// <param name="unregisteredProjections">The <see cref="IUnregisteredProjections"/>.</param>
    /// <param name="eventHorizonsBuilder">The <see cref="SubscriptionsBuilder"/>.</param>
    /// <param name="eventHorizonRetryPolicy">The <see cref="EventSubscriptionRetryPolicy"/>.</param>
    public DolittleClient(
        IClientBuildResults buildResults,
        IUnregisteredEventTypes unregisteredEventTypes,
        IUnregisteredAggregateRoots unregisteredAggregateRoots,
        IUnregisteredEventFilters unregisteredEventFilters,
        IUnregisteredEventHandlers unregisteredEventHandlers,
        IUnregisteredProjections unregisteredProjections,
        SubscriptionsBuilder eventHorizonsBuilder,
        EventSubscriptionRetryPolicy eventHorizonRetryPolicy)
    {
        _buildResults = buildResults;
        _unregisteredEventTypes = unregisteredEventTypes;
        EventTypes = _unregisteredEventTypes;
        _unregisteredAggregateRoots = unregisteredAggregateRoots;
        _unregisteredEventFilters = unregisteredEventFilters;
        _unregisteredEventHandlers = unregisteredEventHandlers;
        _unregisteredProjections = unregisteredProjections;
        _eventHorizonsBuilder = eventHorizonsBuilder;
        _eventHorizonRetryPolicy = eventHorizonRetryPolicy;
    }

    /// <inheritdoc />
    public bool IsConnected { get; private set; }

    /// <inheritdoc />
    public Task Connected => _connectedCompletionSource.Task;

    internal IAggregateRootTypes AggregateRootTypes => _unregisteredAggregateRoots;
    internal IUnregisteredProjections ProjectionTypes => _unregisteredProjections;

    /// <inheritdoc />
    public IEventTypes EventTypes { get; }

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
    public static IDolittleClient Setup(SetupDolittleClient? setup = default)
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
        if (IsConnected)
        {
            throw new CannotConnectDolittleClientMultipleTimes();
        }

        await _connectLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (IsConnected)
            {
                throw new CannotConnectDolittleClientMultipleTimes();
            }

            AddDefaultsFromServiceProviderInConfiguration(configuration);

            var loggerFactory = configuration.LoggerFactory;
            if (loggerFactory is not null)
            {
                _buildResults.WriteTo(loggerFactory.CreateLogger<DolittleClient>());
            }

            _grpcChannel = GrpcChannel.ForAddress(
                $"http://{configuration.RuntimeHost}:{configuration.RuntimePort}",
                new GrpcChannelOptions
                {
                    Credentials = ChannelCredentials.Insecure,
                    MaxReceiveMessageSize = 32 * 1024 * 1024,
                    MaxSendMessageSize = 32 * 1024 * 1024,
#if NET5_0_OR_GREATER
                    HttpHandler = new SocketsHttpHandler
                    {
                        EnableMultipleHttp2Connections = true
                    }
#endif
                });
            var methodCaller = new MethodCaller(_grpcChannel, configuration.RuntimeHost, configuration.RuntimePort);
            var (executionContext, tenants, otlpEndpoint) =
                await ConnectToRuntime(methodCaller, configuration, loggerFactory, cancellationToken).ConfigureAwait(false);
            Tenants = tenants;

            await CreateDependencies(methodCaller, configuration, loggerFactory, executionContext, tenants).ConfigureAwait(false);
            ConfigureContainer(configuration);
            await RegisterAllUnregistered(methodCaller, configuration.PingInterval, executionContext, loggerFactory).ConfigureAwait(false);

            IsConnected = true;
            _connectedCompletionSource.SetResult(true);
            return this;
        }
        finally
        {
            _connectLock.Release();
        }
    }

    Task<ConnectionResult> ConnectToRuntime(IPerformMethodCalls methodCaller, DolittleClientConfiguration configuration, ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        var runtimeConnector = new DolittleRuntimeConnector(
            configuration.RuntimeHost,
            configuration.RuntimePort,
            configuration.Version,
            new HandshakeClient(methodCaller, loggerFactory.CreateLogger<HandshakeClient>()),
            new TenantsClient(methodCaller, loggerFactory.CreateLogger<TenantsClient>()),
            _buildResults,
            loggerFactory.CreateLogger<DolittleRuntimeConnector>());

        return runtimeConnector.ConnectForever(cancellationToken);
    }

    /// <inheritdoc />
    public Task Disconnect(CancellationToken cancellationToken = default)
    {
        _clientCancellationTokenSource.Cancel();
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
            _services?.Dispose();
            _clientCancellationTokenSource?.Dispose();
            _eventHorizons?.Dispose();
            _grpcChannel?.Dispose();
        }

        _disposed = true;
    }

    async Task CreateDependencies(
        IPerformMethodCalls methodCaller,
        DolittleClientConfiguration config,
        ILoggerFactory loggerFactory,
        ExecutionContext executionContext,
        IEnumerable<Tenant> tenants)
    {
        _clientCancellationTokenSource = new CancellationTokenSource();
        var serializer = new EventContentSerializer(_unregisteredEventTypes, config.EventSerializerProvider);
        _eventsToProtobufConverter = new EventToProtobufConverter(serializer);
        _eventToSDKConverter = new EventToSDKConverter(serializer);
        IServiceProvider TenantServiceProvider(TenantId tenant) => Services.ForTenant(tenant);

        EventStore = new EventStoreBuilder(
            methodCaller,
            _eventsToProtobufConverter,
            _eventToSDKConverter,
            new AggregateEventToProtobufConverter(serializer),
            new AggregateEventToSDKConverter(serializer),
            executionContext,
            _callContextResolver,
            _unregisteredEventTypes,
            loggerFactory);
        // Important to not send in the method group because it will crash because it calls get on the Services-property before it is ready.
#pragma warning disable IDE0200
        Aggregates = new AggregatesBuilder(TenantServiceProvider);
#pragma warning restore IDE0200
        EventHorizons = new EventHorizons(
            methodCaller,
            executionContext,
            _eventHorizonRetryPolicy,
            loggerFactory.CreateLogger<EventHorizons>());
        Projections = new ProjectionStoreBuilder(
            TenantServiceProvider,
            executionContext,
            _unregisteredProjections.ReadModelTypes,
            loggerFactory);
        Resources = await new ResourcesFetcher(
            methodCaller,
            executionContext,
            config.ConfigureMongoClientSettings,
            loggerFactory
        ).FetchResourcesFor(tenants, _clientCancellationTokenSource.Token).ConfigureAwait(false);
    }

    async Task RegisterAllUnregistered(IPerformMethodCalls methodCaller, TimeSpan pingInterval, ExecutionContext executionContext, ILoggerFactory loggerFactory)
    {
        _eventHorizonsBuilder.BuildAndSubscribe(_eventHorizons, _clientCancellationTokenSource.Token);
        await _unregisteredEventTypes.Register(
            new Events.Internal.EventTypesClient(
                methodCaller,
                executionContext,
                loggerFactory.CreateLogger<Events.Internal.EventTypesClient>()),
            _clientCancellationTokenSource.Token).ConfigureAwait(false);
        await _unregisteredAggregateRoots.Register(
            new AggregateRootsClient(
                methodCaller,
                executionContext,
                loggerFactory.CreateLogger<AggregateRootsClient>()),
            _clientCancellationTokenSource.Token).ConfigureAwait(false);
        StartEventProcessors(methodCaller, pingInterval, executionContext, loggerFactory);
    }

    void StartEventProcessors(IPerformMethodCalls methodCaller, TimeSpan pingInterval, ExecutionContext executionContext, ILoggerFactory loggerFactory)
    {
        var reverseCallClientsCreator = new ReverseCallClientCreator(
            pingInterval,
            methodCaller,
            executionContext,
            _services,
            loggerFactory);
        var eventProcessors = new EventProcessors(
            reverseCallClientsCreator,
            _processingCoordinator,
            loggerFactory.CreateLogger<EventProcessors>());

        var eventProcessingConverter = new EventProcessingConverter(_eventToSDKConverter);
        _unregisteredEventHandlers.Register(
            eventProcessors,
            eventProcessingConverter,
            loggerFactory,
            _clientCancellationTokenSource.Token);
        _unregisteredEventFilters.Register(
            eventProcessors,
            eventProcessingConverter,
            loggerFactory,
            _clientCancellationTokenSource.Token);
        _unregisteredProjections.Register(
            eventProcessors,
            eventProcessingConverter,
            loggerFactory,
            _clientCancellationTokenSource.Token);
    }

    TRequiresStartService GetOrThrowIfNotConnected<TRequiresStartService>(TRequiresStartService service)
    {
        if (!IsConnected)
        {
            throw new CannotUseUnconnectedDolittleClient();
        }

        return service;
    }

    static void AddDefaultsFromServiceProviderInConfiguration(DolittleClientConfiguration config)
    {
        var serviceProvider = config.ServiceProvider;
        if (serviceProvider is null)
        {
            return;
        }

        if (config.LoggerFactory is null)
        {
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            config.WithLogging(loggerFactory ?? LoggerFactory.Create(_ =>
            {
                _.SetMinimumLevel(LogLevel.Information);
                _.AddConsole();
            }));
        }

        if (config.TenantServiceProviderFactory is null)
        {
            var providerFactory = serviceProvider.GetService<ICreateTenantContainers>();
            config.WithTenantServiceProviderFactory(providerFactory is not null
                ? providerFactory.Create
                : DefaultTenantServiceProviderFactory.Instance);
        }
    }

    void ConfigureContainer(DolittleClientConfiguration config)
    {
        var builder = new TenantScopedProvidersBuilder(config.ServiceProvider, config.TenantServiceProviderFactory)
            .AddTenantServices(AddBuilderServices)
            .AddTenantServices((_, collection) => collection.AddScoped(services => services.GetRequiredService<IResources>().MongoDB.GetDatabase(config.ConfigureMongoDatabaseSettings)))
            .AddTenantServices(_unregisteredEventHandlers.AddTenantScopedServices)
            .AddTenantServices(_unregisteredAggregateRoots.AddTenantScopedServices)
            .AddTenantServices(_unregisteredProjections.AddTenantScopedServices)
            .AddTenantServices(config.ConfigureTenantServices);
        Services = builder.Build(_tenants.Select(_ => _.Id).ToImmutableHashSet());
    }

    void AddBuilderServices(TenantId tenant, IServiceCollection services)
        => services
            .AddScoped(_ => EventStore.ForTenant(tenant))
            .AddScoped(_ => Aggregates.ForTenant(tenant))
            .AddScoped(_ => Projections.ForTenant(tenant))
            .AddScoped(_ => Resources.ForTenant(tenant));
}

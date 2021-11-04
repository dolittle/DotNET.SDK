// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using BaselineTypeDiscovery;
using Dolittle.SDK.Aggregates.Builders;
using Dolittle.SDK.Aggregates.Internal;
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
using Dolittle.SDK.Security;
using Dolittle.SDK.Services;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Environment = Dolittle.SDK.Microservices.Environment;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;
using Version = Dolittle.SDK.Microservices.Version;

namespace Dolittle.SDK
{
    /// <summary>
    /// Represents a builder for building <see cref="Client" />.
    /// </summary>
    public class ClientBuilder
    {
        readonly EventTypesBuilder _eventTypesBuilder;
        readonly AggregateRootsBuilder _aggregateRootsBuilder;
        readonly EventFiltersBuilder _eventFiltersBuilder;
        readonly EventHandlersBuilder _eventHandlersBuilder;
        readonly ProjectionsBuilder _projectionsBuilder;
        readonly SubscriptionsBuilder _eventHorizonsBuilder;
        readonly EmbeddingsBuilder _embeddingsBuilder;
        readonly MicroserviceId _microserviceId;
        readonly ProjectionReadModelTypeAssociations _projectionAssociations;
        readonly EmbeddingReadModelTypeAssociations _embeddingAssociations;
        readonly EventSubscriptionRetryPolicy _eventHorizonRetryPolicy;
        string _host = "localhost";
        TimeSpan _pingInterval = TimeSpan.FromSeconds(5);
        ushort _port = 50053;
        Version _version;
        Environment _environment;
        CancellationToken _cancellation;
        Action<JsonSerializerSettings> _jsonSerializerSettingsBuilder;

        ILoggerFactory _loggerFactory = LoggerFactory.Create(_ =>
            {
                _.SetMinimumLevel(LogLevel.Information);
                _.AddConsole();
            });

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientBuilder"/> class.
        /// </summary>
        /// <param name="microserviceId">The <see cref="MicroserviceId"/> of the microservice.</param>
        public ClientBuilder(MicroserviceId microserviceId)
        {
            _microserviceId = microserviceId;

            _version = Version.NotSet;
            _environment = Environment.Undetermined;
            _cancellation = CancellationToken.None;
            _eventHorizonRetryPolicy = EventHorizonRetryPolicy;

            _projectionAssociations = new ProjectionReadModelTypeAssociations();
            _embeddingAssociations = new EmbeddingReadModelTypeAssociations();

            _eventTypesBuilder = new EventTypesBuilder();
            _aggregateRootsBuilder = new AggregateRootsBuilder();
            _eventFiltersBuilder = new EventFiltersBuilder();
            _eventHandlersBuilder = new EventHandlersBuilder();
            _projectionsBuilder = new ProjectionsBuilder(_projectionAssociations);
            _eventHorizonsBuilder = new SubscriptionsBuilder();
            _embeddingsBuilder = new EmbeddingsBuilder(_embeddingAssociations);
        }

        /// <summary>
        /// Sets the version of the microservice.
        /// </summary>
        /// <param name="version">The version of the microservice.</param>
        /// <returns>The client builder for continuation.</returns>
        public ClientBuilder WithVersion(Version version)
        {
            _version = version;
            return this;
        }

        /// <summary>
        /// Sets the ping interval for communicating with the microservice.
        /// </summary>
        /// <param name="interval">The ping interval.</param>
        /// <returns>The client builder for continuation.</returns>
        public ClientBuilder WithPingInterval(TimeSpan interval)
        {
            _pingInterval = interval;
            return this;
        }

        /// <summary>
        /// Sets the version of the microservice.
        /// </summary>
        /// <param name="major">Major version of the microservice.</param>
        /// <param name="minor">Minor version of the microservice.</param>
        /// <param name="patch">Path level of the microservice.</param>
        /// <param name="build">Build number of the microservice.</param>
        /// <param name="preReleaseString">If prerelease - the prerelease string.</param>
        /// <returns>The client builder for continuation.</returns>
        public ClientBuilder WithVersion(int major, int minor, int patch, int build = 0, string preReleaseString = "")
        {
            _version = new Version(major, minor, patch, build, preReleaseString);
            return this;
        }

        /// <summary>
        /// Sets the environment in which the microservice is running.
        /// </summary>
        /// <param name="environment">The environment in which the microservice is running.</param>
        /// <returns>The client builder for continuation.</returns>
        public ClientBuilder WithEnvironment(Environment environment)
        {
            _environment = environment;
            return this;
        }

        /// <summary>
        /// Connect to a specific host and port for the Dolittle runtime.
        /// </summary>
        /// <param name="host">The host name to connect to.</param>
        /// <param name="port">The port to connect to.</param>
        /// <returns>The client builder for continuation.</returns>
        /// <remarks>If not specified, host 'localhost' and port 50053 will be used.</remarks>
        public ClientBuilder WithRuntimeOn(string host, ushort port)
        {
            _host = host;
            _port = port;
            return this;
        }

        /// <summary>
        /// Sets the cancellation token for cancelling pending operations on the Runtime.
        /// </summary>
        /// <param name="cancellation">The cancellation token for cancelling pending operations on the Runtime.</param>
        /// <returns>The client builder for continuation.</returns>
        public ClientBuilder WithCancellation(CancellationToken cancellation)
        {
            _cancellation = cancellation;
            return this;
        }

        /// <summary>
        /// Registers all artifacts by scanning all assemblies.
        /// </summary>
        /// <param name="assemblyFilter">The filter for assemblies.</param>
        /// <param name="includeExeFiles">Whether or not to also load assemblies from .exe files.</param>
        /// <returns>The client builder for continuation.</returns>
        public ClientBuilder WithAllArtifacts(Func<Assembly, bool> assemblyFilter = default, bool includeExeFiles = false)
            => WithAllScannedAssemblies(
                asm =>
                {
                    _embeddingsBuilder.RegisterAllFrom(asm);
                    _projectionsBuilder.RegisterAllFrom(asm);
                    _aggregateRootsBuilder.RegisterAllFrom(asm);
                    _eventHandlersBuilder.RegisterAllFrom(asm);
                    _eventTypesBuilder.RegisterAllFrom(asm);
                },
                assemblyFilter,
                includeExeFiles);

        /// <summary>
        /// Sets the event types through the <see cref="EventTypesBuilder" />.
        /// </summary>
        /// <param name="callback">The builder callback.</param>
        /// <returns>The client builder for continuation.</returns>
        public ClientBuilder WithEventTypes(Action<EventTypesBuilder> callback)
        {
            callback(_eventTypesBuilder);
            return this;
        }

        /// <summary>
        /// Registers all event types by scanning all assemblies.
        /// </summary>
        /// <param name="assemblyFilter">The filter for assemblies.</param>
        /// <param name="includeExeFiles">Whether or not to also load assemblies from .exe files.</param>
        /// <returns>The client builder for continuation.</returns>
        public ClientBuilder WithEventTypes(Func<Assembly, bool> assemblyFilter = default, bool includeExeFiles = false)
            => WithAllScannedAssemblies(asm => _eventTypesBuilder.RegisterAllFrom(asm), assemblyFilter, includeExeFiles);

        /// <summary>
        /// Sets the aggregate roots through the <see cref="EventTypesBuilder" />.
        /// </summary>
        /// <param name="callback">The builder callback.</param>
        /// <returns>The client builder for continuation.</returns>
        public ClientBuilder WithAggregateRoots(Action<AggregateRootsBuilder> callback)
        {
            callback(_aggregateRootsBuilder);
            return this;
        }

        /// <summary>
        /// Registers all aggregate roots by scanning all assemblies.
        /// </summary>
        /// <param name="assemblyFilter">The filter for assemblies.</param>
        /// <param name="includeExeFiles">Whether or not to also load assemblies from .exe files.</param>
        /// <returns>The client builder for continuation.</returns>
        public ClientBuilder WithAggregateRoots(Func<Assembly, bool> assemblyFilter = default, bool includeExeFiles = false)
            => WithAllScannedAssemblies(asm => _aggregateRootsBuilder.RegisterAllFrom(asm), assemblyFilter, includeExeFiles);

        /// <summary>
        /// Sets the filters through the <see cref="EventFiltersBuilder" />.
        /// </summary>
        /// <param name="callback">The builder callback.</param>
        /// <returns>The client builder for continuation.</returns>
        public ClientBuilder WithFilters(Action<EventFiltersBuilder> callback)
        {
            callback(_eventFiltersBuilder);
            return this;
        }

        /// <summary>
        /// Sets the event handlers through the <see cref="EventHandlersBuilder" />.
        /// </summary>
        /// <param name="callback">The builder callback.</param>
        /// <returns>The client builder for continuation.</returns>
        public ClientBuilder WithEventHandlers(Action<EventHandlersBuilder> callback)
        {
            callback(_eventHandlersBuilder);
            return this;
        }

        /// <summary>
        /// Registers all event handlers by scanning all assemblies.
        /// </summary>
        /// <param name="assemblyFilter">The filter for assemblies.</param>
        /// <param name="includeExeFiles">Whether or not to also load assemblies from .exe files.</param>
        /// <returns>The client builder for continuation.</returns>
        public ClientBuilder WithEventHandlers(Func<Assembly, bool> assemblyFilter = default, bool includeExeFiles = false)
            => WithAllScannedAssemblies(asm => _eventHandlersBuilder.RegisterAllFrom(asm), assemblyFilter, includeExeFiles);

        /// <summary>
        /// Sets the event handlers through the <see cref="ProjectionsBuilder" />.
        /// </summary>
        /// <param name="callback">The builder callback.</param>
        /// <returns>The client builder for continuation.</returns>
        public ClientBuilder WithProjections(Action<ProjectionsBuilder> callback)
        {
            callback(_projectionsBuilder);
            return this;
        }

        /// <summary>
        /// Registers all projections by scanning all assemblies.
        /// </summary>
        /// <param name="assemblyFilter">The filter for assemblies.</param>
        /// <param name="includeExeFiles">Whether or not to also load assemblies from .exe files.</param>
        /// <returns>The client builder for continuation.</returns>
        public ClientBuilder WithProjections(Func<Assembly, bool> assemblyFilter = default, bool includeExeFiles = false)
            => WithAllScannedAssemblies(asm => _projectionsBuilder.RegisterAllFrom(asm), assemblyFilter, includeExeFiles);

        /// <summary>
        /// Sets the embeddings through the <see cref="EmbeddingsBuilder" />.
        /// </summary>
        /// <param name="callback">The builder callback.</param>
        /// <returns>The client builder for continuation.</returns>
        public ClientBuilder WithEmbeddings(Action<EmbeddingsBuilder> callback)
        {
            callback(_embeddingsBuilder);
            return this;
        }

        /// <summary>
        /// Registers all embeddings by scanning all assemblies.
        /// </summary>
        /// <param name="assemblyFilter">The filter for assemblies.</param>
        /// <param name="includeExeFiles">Whether or not to also load assemblies from .exe files.</param>
        /// <returns>The client builder for continuation.</returns>
        public ClientBuilder WithEmbeddings(Func<Assembly, bool> assemblyFilter = default, bool includeExeFiles = false)
            => WithAllScannedAssemblies(asm => _embeddingsBuilder.RegisterAllFrom(asm), assemblyFilter, includeExeFiles);

        /// <summary>
        /// Sets the event handlers through the <see cref="SubscriptionsBuilder" />.
        /// </summary>
        /// <param name="callback">The builder callback.</param>
        /// <returns>The client builder for continuation.</returns>
        public ClientBuilder WithEventHorizons(Action<SubscriptionsBuilder> callback)
        {
            callback(_eventHorizonsBuilder);
            return this;
        }

        /// <summary>
        /// Sets the <see cref="ILoggerFactory"/> to use for creating instances of <see cref="ILogger"/> for the client.
        /// </summary>
        /// <param name="factory">The given <see cref="ILoggerFactory"/>.</param>
        /// <returns>The client builder for continuation.</returns>
        /// <remarks>If not used, a factory with 'Trace' level logging will be used.</remarks>
        public ClientBuilder WithLogging(ILoggerFactory factory)
        {
            _loggerFactory = factory;
            return this;
        }

        /// <summary>
        /// Sets a callback that configures the <see cref="JsonSerializerSettings"/> for serializing events.
        /// </summary>
        /// <param name="jsonSerializerSettingsBuilder"><see cref="Action{T}"/> that gets called with <see cref="JsonSerializerSettings"/> to modify settings.</param>
        /// <returns>The client builder for continuation.</returns>
        public ClientBuilder WithEventSerializerSettings(Action<JsonSerializerSettings> jsonSerializerSettingsBuilder)
        {
            _jsonSerializerSettingsBuilder = jsonSerializerSettingsBuilder;
            return this;
        }

        /// <summary>
        /// Build the Client.
        /// </summary>
        /// <returns>The <see cref="Client"/>.</returns>
        public Client Build()
        {
            var methodCaller = new MethodCaller(_host, _port);
            var executionContext = new ExecutionContext(
                _microserviceId,
                TenantId.System,
                _version,
                _environment,
                CorrelationId.System,
                Claims.Empty,
                CultureInfo.InvariantCulture);
            var eventTypes = new EventTypes(_loggerFactory.CreateLogger<EventTypes>());
            _eventTypesBuilder.AddAssociationsInto(eventTypes);
            _eventTypesBuilder.BuildAndRegister(new Events.Internal.EventTypesClient(methodCaller, executionContext, _loggerFactory.CreateLogger<Events.Internal.EventTypesClient>()), _cancellation);
            _aggregateRootsBuilder.BuildAndRegister(new AggregateRootsClient(methodCaller, executionContext, _loggerFactory.CreateLogger<AggregateRoots>()), _cancellation);

            var reverseCallClientsCreator = new ReverseCallClientCreator(
                _pingInterval,
                methodCaller,
                executionContext,
                _loggerFactory);

            Func<JsonSerializerSettings> jsonSerializerSettingsProvider = () =>
            {
                var settings = new JsonSerializerSettings();
                _jsonSerializerSettingsBuilder?.Invoke(settings);
                return settings;
            };

            var serializer = new EventContentSerializer(
                eventTypes,
                jsonSerializerSettingsProvider);
            var eventToProtobufConverter = new EventToProtobufConverter(serializer);
            var eventToSDKConverter = new EventToSDKConverter(serializer);
            var aggregateEventToProtobufConverter = new AggregateEventToProtobufConverter(serializer);
            var aggregateEventToSDKConverter = new AggregateEventToSDKConverter(serializer);
            var projectionsToSDKConverter = new ProjectionsToSDKConverter();

            var eventProcessingConverter = new EventProcessingConverter(eventToSDKConverter);
            var processingCoordinator = new ProcessingCoordinator();

            var eventProcessors = new EventProcessors(reverseCallClientsCreator, processingCoordinator, _loggerFactory.CreateLogger<EventProcessors>());

            var callContextResolver = new CallContextResolver();

            var eventStoreBuilder = new EventStoreBuilder(
                methodCaller,
                eventToProtobufConverter,
                eventToSDKConverter,
                aggregateEventToProtobufConverter,
                aggregateEventToSDKConverter,
                executionContext,
                callContextResolver,
                eventTypes,
                _loggerFactory);

            var eventHorizons = new EventHorizons(methodCaller, executionContext, _eventHorizonRetryPolicy, _loggerFactory.CreateLogger<EventHorizons>());
            _eventHorizonsBuilder.BuildAndSubscribe(eventHorizons, _cancellation);

            var projectionStoreBuilder = new ProjectionStoreBuilder(
                methodCaller,
                executionContext,
                callContextResolver,
                _projectionAssociations,
                projectionsToSDKConverter,
                _loggerFactory);
            var embeddings = new Embeddings.Embeddings(
                methodCaller,
                callContextResolver,
                _embeddingAssociations,
                projectionsToSDKConverter,
                executionContext,
                _loggerFactory);

            return new Client(
                eventTypes,
                eventStoreBuilder,
                eventHorizons,
                processingCoordinator,
                eventProcessors,
                eventProcessingConverter,
                _eventHandlersBuilder,
                _eventFiltersBuilder,
                projectionsToSDKConverter,
                eventToProtobufConverter,
                _projectionsBuilder,
                _embeddingsBuilder,
                projectionStoreBuilder,
                embeddings,
                _loggerFactory,
                _cancellation);
        }

        static async Task EventHorizonRetryPolicy(Subscription subscription, ILogger logger, Func<Task<bool>> methodToPerform)
        {
            var retryCount = 0;

            while (!await methodToPerform().ConfigureAwait(false))
            {
                retryCount++;
                var timeout = TimeSpan.FromSeconds(5);
                logger.LogDebug(
                    "Retry attempt {retryCount} processing subscription to events in {Timeout}ms () from {ProducerMicroservice} in {ProducerTenant} in {ProducerStream} in {ProducerPartition} for {ConsumerTenant} into {ConsumerScope}",
                    retryCount,
                    timeout,
                    subscription.ProducerMicroservice,
                    subscription.ProducerTenant,
                    subscription.ProducerStream,
                    subscription.ProducerPartition,
                    subscription.ConsumerTenant,
                    subscription.ConsumerScope);

                await Task.Delay(timeout).ConfigureAwait(false);
            }
        }

        ClientBuilder WithAllScannedAssemblies(Action<Assembly> doWithAssembly, Func<Assembly, bool> assemblyFilter = default, bool includeExeFiles = false)
        {
            foreach (var assembly in GetAssemblies(assemblyFilter, includeExeFiles))
            {
                doWithAssembly(assembly);
            }

            return this;
        }

        IEnumerable<Assembly> GetAssemblies(Func<Assembly, bool> assemblyFilter, bool includeExeFiles)
        {
            var logger = _loggerFactory.CreateLogger<ClientBuilder>();
            return AssemblyFinder.FindAssemblies(
                failedFile => logger.LogWarning("Failed to load assembly from file {File}", failedFile),
                assemblyFilter ?? (_ => true),
                includeExeFiles);
        }
    }
}

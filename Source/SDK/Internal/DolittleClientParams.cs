// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Aggregates.Builders;
using Dolittle.SDK.Aggregates.Internal;
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
using Dolittle.SDK.Tenancy.Client;
using Dolittle.SDK.Tenancy.Client.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Environment = Dolittle.SDK.Microservices.Environment;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;
using Version = Dolittle.SDK.Microservices.Version;

namespace Dolittle.SDK.Internal
{
#pragma warning disable CS1591
    public class DolittleClientParams
    {
        public static async Task<DolittleClientParams> Create(
#pragma warning restore CS1591
            string host,
            ushort port,
            MicroserviceId microserviceId,
            Version version,
            Environment environment,
            TimeSpan pingInterval,
            ProjectionReadModelTypeAssociations projectionAssociations,
            EmbeddingReadModelTypeAssociations embeddingAssociations,
            EventSubscriptionRetryPolicy eventHorizonRetryPolicy,
            EventTypesBuilder eventTypesBuilder,
            AggregateRootsBuilder aggregateRootsBuilder,
            EmbeddingsBuilder embeddingsBuilder,
            ProjectionsBuilder projectionsBuilder,
            EventHandlersBuilder eventHandlersBuilder,
            EventFiltersBuilder filtersBuilder,
            SubscriptionsBuilder eventHorizonsBuilder,
            Action<JsonSerializerSettings> jsonSerializerSettingsBuilder,
            ILoggerFactory loggerFactory,
            CancellationToken cancellation)
        {
            var methodCaller = new MethodCaller(host, port);
            var executionContext = new ExecutionContext(
                microserviceId,
                TenantId.System,
                version,
                environment,
                CorrelationId.System,
                Claims.Empty,
                CultureInfo.InvariantCulture);
            var eventTypes = new EventTypes(loggerFactory.CreateLogger<EventTypes>());
            eventTypesBuilder.AddAssociationsInto(eventTypes);
            await eventTypesBuilder.BuildAndRegister(new Events.Internal.EventTypesClient(methodCaller, executionContext, loggerFactory.CreateLogger<Events.Internal.EventTypesClient>()), cancellation).ConfigureAwait(false);
            await aggregateRootsBuilder.BuildAndRegister(new AggregateRootsClient(methodCaller, executionContext, loggerFactory.CreateLogger<AggregateRoots>()), cancellation).ConfigureAwait(false);

            var reverseCallClientsCreator = new ReverseCallClientCreator(
                pingInterval,
                methodCaller,
                executionContext,
                loggerFactory);

            Func<JsonSerializerSettings> jsonSerializerSettingsProvider = () =>
            {
                var settings = new JsonSerializerSettings();
                jsonSerializerSettingsBuilder?.Invoke(settings);
                return settings;
            };

            var serializer = new EventContentSerializer(
                eventTypes,
                jsonSerializerSettingsProvider);
            var eventToProtobufConverter = new EventToProtobufConverter(serializer);
            var eventToSDKConverter = new EventToSDKConverter(serializer);
            var aggregateEventToProtobufConverter = new AggregateEventToProtobufConverter(serializer);
            var aggregateEventToSDKConverter = new AggregateEventToSDKConverter(serializer);
            var projectionConverter = new ProjectionsToSDKConverter();

            var eventProcessingConverter = new EventProcessingConverter(eventToSDKConverter);
            var processingCoordinator = new ProcessingCoordinator();

            var eventProcessors = new EventProcessors(reverseCallClientsCreator, processingCoordinator, loggerFactory.CreateLogger<EventProcessors>());

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
                loggerFactory);

            var eventHorizons = new EventHorizons(methodCaller, executionContext, eventHorizonRetryPolicy, loggerFactory.CreateLogger<EventHorizons>());
            eventHorizonsBuilder.BuildAndSubscribe(eventHorizons, cancellation);

            var projectionStoreBuilder = new ProjectionStoreBuilder(
                methodCaller,
                executionContext,
                callContextResolver,
                projectionAssociations,
                projectionConverter,
                loggerFactory);
            var embeddings = new Embeddings.Embeddings(
                methodCaller,
                callContextResolver,
                embeddingAssociations,
                projectionConverter,
                executionContext,
                loggerFactory);

            var aggregateRoots = new AggregateRoots(loggerFactory.CreateLogger<AggregateRoots>());

            var tenants = new TenantsClient(methodCaller, executionContext, loggerFactory.CreateLogger<TenantsClient>());
            var resources = new ResourcesBuilder(methodCaller, executionContext, loggerFactory);

            return new DolittleClientParams
            {
                Embeddings = embeddings,
                Resources = resources,
                Tenants = await tenants.GetAll(cancellation).ConfigureAwait(false),
                AggregateRoots = aggregateRoots,
                CancellationToken = cancellation,
                EmbeddingsBuilder = embeddingsBuilder,
                EventHorizons = eventHorizons,
                EventProcessors = eventProcessors,
                EventTypes = eventTypes,
                FiltersBuilder = filtersBuilder,
                LoggerFactory = loggerFactory,
                ProcessingCoordinator = processingCoordinator,
                ProjectionConverter = projectionConverter,
                ProjectionsBuilder = projectionsBuilder,
                EventHandlersBuilder = eventHandlersBuilder,
                EventProcessingConverter = eventProcessingConverter,
                EventStoreBuilder = eventStoreBuilder,
                ProjectionStoreBuilder = projectionStoreBuilder,
                EventsToProtobufConverter = eventToProtobufConverter
            };
        }

#pragma warning disable CS1591
        public IEventTypes EventTypes { get; set; }
        public IEventStoreBuilder EventStoreBuilder { get; set; }
        public EventHorizons EventHorizons { get; set; }
        public ProcessingCoordinator ProcessingCoordinator { get; set; }
        public IEventProcessors EventProcessors { get; set; }
        public IEventProcessingConverter EventProcessingConverter { get; set; }
        public EventHandlersBuilder EventHandlersBuilder { get; set; }
        public EventFiltersBuilder FiltersBuilder { get; set; }
        public IConvertProjectionsToSDK ProjectionConverter { get; set; }
        public IConvertEventsToProtobuf EventsToProtobufConverter { get; set; }
        public ProjectionsBuilder ProjectionsBuilder { get; set; }
        public EmbeddingsBuilder EmbeddingsBuilder { get; set; }
        public IProjectionStoreBuilder ProjectionStoreBuilder { get; set; }
        public IEmbeddings Embeddings { get; set; }
        public IAggregateRoots AggregateRoots { get; set; }
        public IEnumerable<Tenant> Tenants { get; set; }
        public IResourcesBuilder Resources { get; set; }
        public ILoggerFactory LoggerFactory { get; set; }
        public CancellationToken CancellationToken { get; set; }
#pragma warning restore CS1591
    }
}
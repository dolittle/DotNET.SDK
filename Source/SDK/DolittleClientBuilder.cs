// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.SDK.Aggregates.Builders;
using Dolittle.SDK.Embeddings.Builder;
using Dolittle.SDK.Embeddings.Store;
using Dolittle.SDK.EventHorizon;
using Dolittle.SDK.Events.Builders;
using Dolittle.SDK.Events.Filters;
using Dolittle.SDK.Events.Handling.Builder;
using Dolittle.SDK.Projections.Builder;
using Dolittle.SDK.Projections.Store;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK
{
    /// <summary>
    /// Represents a builder for building <see cref="DolittleClient" />.
    /// </summary>
    public class DolittleClientBuilder
    {
        readonly EventTypesBuilder _eventTypesBuilder = new EventTypesBuilder();
        readonly AggregateRootsBuilder _aggregateRootsBuilder = new AggregateRootsBuilder();
        readonly EventFiltersBuilder _eventFiltersBuilder = new EventFiltersBuilder();
        readonly EventHandlersBuilder _eventHandlersBuilder = new EventHandlersBuilder();
        readonly SubscriptionsBuilder _eventHorizonsBuilder = new SubscriptionsBuilder();
        readonly ProjectionReadModelTypeAssociations _projectionAssociations = new ProjectionReadModelTypeAssociations();
        readonly EmbeddingReadModelTypeAssociations _embeddingAssociations = new EmbeddingReadModelTypeAssociations();
        readonly EventSubscriptionRetryPolicy _eventHorizonRetryPolicy = EventHorizonRetryPolicy;
        readonly ProjectionsBuilder _projectionsBuilder;
        readonly EmbeddingsBuilder _embeddingsBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="DolittleClientBuilder"/> class.
        /// </summary>
        public DolittleClientBuilder()
        {
            _projectionsBuilder = new ProjectionsBuilder(_projectionAssociations);
            _embeddingsBuilder = new EmbeddingsBuilder(_embeddingAssociations);
        }

        /// <summary>
        /// Sets the event types through the <see cref="EventTypesBuilder" />.
        /// </summary>
        /// <param name="callback">The builder callback.</param>
        /// <returns>The client builder for continuation.</returns>
        public DolittleClientBuilder WithEventTypes(Action<EventTypesBuilder> callback)
        {
            callback(_eventTypesBuilder);
            return this;
        }

        /// <summary>
        /// Sets the aggregate roots through the <see cref="EventTypesBuilder" />.
        /// </summary>
        /// <param name="callback">The builder callback.</param>
        /// <returns>The client builder for continuation.</returns>
        public DolittleClientBuilder WithAggregateRoots(Action<AggregateRootsBuilder> callback)
        {
            callback(_aggregateRootsBuilder);
            return this;
        }

        /// <summary>
        /// Sets the filters through the <see cref="EventFiltersBuilder" />.
        /// </summary>
        /// <param name="callback">The builder callback.</param>
        /// <returns>The client builder for continuation.</returns>
        public DolittleClientBuilder WithFilters(Action<EventFiltersBuilder> callback)
        {
            callback(_eventFiltersBuilder);
            return this;
        }

        /// <summary>
        /// Sets the event handlers through the <see cref="EventHandlersBuilder" />.
        /// </summary>
        /// <param name="callback">The builder callback.</param>
        /// <returns>The client builder for continuation.</returns>
        public DolittleClientBuilder WithEventHandlers(Action<EventHandlersBuilder> callback)
        {
            callback(_eventHandlersBuilder);
            return this;
        }

        /// <summary>
        /// Sets the event handlers through the <see cref="ProjectionsBuilder" />.
        /// </summary>
        /// <param name="callback">The builder callback.</param>
        /// <returns>The client builder for continuation.</returns>
        public DolittleClientBuilder WithProjections(Action<ProjectionsBuilder> callback)
        {
            callback(_projectionsBuilder);
            return this;
        }

        /// <summary>
        /// Sets the embeddings through the <see cref="EmbeddingsBuilder" />.
        /// </summary>
        /// <param name="callback">The builder callback.</param>
        /// <returns>The client builder for continuation.</returns>
        public DolittleClientBuilder WithEmbeddings(Action<EmbeddingsBuilder> callback)
        {
            callback(_embeddingsBuilder);
            return this;
        }

        /// <summary>
        /// Sets the event horizons through the <see cref="SubscriptionsBuilder" />.
        /// </summary>
        /// <param name="callback">The builder callback.</param>
        /// <returns>The client builder for continuation.</returns>
        public DolittleClientBuilder WithEventHorizons(Action<SubscriptionsBuilder> callback)
        {
            callback(_eventHorizonsBuilder);
            return this;
        }

        /// <summary>
        /// Builds an unconnected <see cref="DolittleClient"/>.
        /// </summary>
        /// <returns>The <see cref="DolittleClient"/>.</returns>
        public IDolittleClient Build()
            => new DolittleClient(
                _eventTypesBuilder,
                _projectionAssociations,
                _embeddingAssociations,
                _eventHandlersBuilder,
                _aggregateRootsBuilder,
                _projectionsBuilder,
                _embeddingsBuilder,
                _eventFiltersBuilder,
                _eventHorizonsBuilder,
                _eventHorizonRetryPolicy);

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
    }
}

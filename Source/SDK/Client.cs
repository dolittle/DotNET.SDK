// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.DependencyInversion;
using Dolittle.SDK.EventHorizon;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Filters;
using Dolittle.SDK.Events.Handling.Builder;
using Dolittle.SDK.Events.Processing;
using Dolittle.SDK.Events.Store;
using Dolittle.SDK.Events.Store.Builders;
using Dolittle.SDK.Microservices;
using Dolittle.SDK.Projections.Builder;
using Dolittle.SDK.Services;
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
        readonly ProjectionsBuilder _projectionsBuilder;
        readonly EventHorizons _eventHorizons;
        readonly IEventProcessors _eventProcessors;
        readonly IEventProcessingConverter _eventProcessingConverter;
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
        /// <param name="projectionsBuilder">The <see cref="ProjectionsBuilder" />.</param>
        /// <param name="projectionStoreBuilder">The <see cref="Projections.Store.Builders.ProjectionsBuilder" />.</param>
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
            ProjectionsBuilder projectionsBuilder,
            Projections.Store.Builders.ProjectionsBuilder projectionStoreBuilder,
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
            _projectionsBuilder = projectionsBuilder;
            Projections = projectionStoreBuilder;
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
        /// Gets the <see cref="Projections.Store.Builders.ProjectionsBuilder" />.
        /// </summary>
        public Projections.Store.Builders.ProjectionsBuilder Projections { get; }

        /// <summary>
        /// Gets the <see cref="IEventHorizons" />.
        /// </summary>
        public IEventHorizons EventHorizons => _eventHorizons;

        /// <summary>
        /// Create a client builder for a Miroservice.
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
            => new AggregateOf<TAggregateRoot>(buildEventStore(EventStore), EventTypes, _loggerFactory)
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
            => new AggregateOf<TAggregateRoot>(buildEventStore(EventStore), EventTypes, _loggerFactory)
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
                _processingCoordinator.Dispose();
            }

            _disposed = true;
        }
    }
}

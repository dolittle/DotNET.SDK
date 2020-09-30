// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.SDK.EventHorizon;
using Dolittle.SDK.Events;
using Dolittle.SDK.Microservices;
using Dolittle.SDK.Services;
using Environment = Dolittle.SDK.Microservices.Environment;
using Version = Dolittle.SDK.Microservices.Version;

namespace Dolittle.SDK
{
    /// <summary>
    /// Represents the client for working with the Dolittle Runtime.
    /// </summary>
    public class Client : IDisposable
    {
        readonly ProcessingCoordinator _processingCoordinator;
        bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="Client" /> class.
        /// </summary>
        /// <param name="eventTypes">The <see cref="EventTypes" />.</param>
        /// <param name="eventStoreBuilder">The <see cref="EventStoreBuilder" />.</param>
        /// <param name="eventHorizons">The <see cref="IEventHorizons" />.</param>
        /// <param name="processingCoordinator">The <see cref="ProcessingCoordinator" />.</param>
        public Client(
            IEventTypes eventTypes,
            EventStoreBuilder eventStoreBuilder,
            IEventHorizons eventHorizons,
            ProcessingCoordinator processingCoordinator)
        {
            EventTypes = eventTypes;
            EventStore = eventStoreBuilder;
            EventHorizons = eventHorizons;
            _processingCoordinator = processingCoordinator;
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
        /// Gets the <see cref="IEventHorizons" />.
        /// </summary>
        public IEventHorizons EventHorizons { get; }

        /// <summary>
        /// Create a client builder for a Miroservice.
        /// </summary>
        /// <param name="microserviceId">The unique identifier for the microservice.</param>
        /// <returns>The <see cref="ClientBuilder"/> to build the <see cref="Client"/> from.</returns>
        public static ClientBuilder ForMicroservice(MicroserviceId microserviceId)
            => new ClientBuilder(microserviceId, Version.NotSet, Environment.Undetermined);

        /// <summary>
        /// Runs the client until the <see cref="CancellationToken"/> given in
        /// <see cref="ClientBuilder.WithCancellationToken(CancellationToken)"/> is cancelled.
        /// </summary>
        public void Wait()
            => _processingCoordinator.Completion.Wait();

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
                _processingCoordinator.Dispose();
            }

            _disposed = true;
        }
    }
}

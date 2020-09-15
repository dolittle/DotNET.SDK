// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Microservices;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK
{
    /// <summary>
    /// Represents the client for working with the Dolittle Runtime.
    /// </summary>
    public class Client
    {
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Client" /> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        /// <param name="executionContextManager">The <see cref="IExecutionContextManager" />.</param>
        /// <param name="eventTypes">The <see cref="Events.EventTypes" />.</param>
        /// <param name="eventStore">The <see cref="Events.EventStore" />.</param>
        public Client(
            ILogger logger,
            IExecutionContextManager executionContextManager,
            IEventTypes eventTypes,
            IEventStore eventStore)
        {
            _logger = logger;
            EventTypes = eventTypes;
            ExecutionContextManager = executionContextManager;
            EventStore = eventStore;
        }

        /// <summary>
        /// Gets the <see cref="IExecutionContextManager" />.
        /// </summary>
        public IExecutionContextManager ExecutionContextManager { get; }

        /// <summary>
        /// Gets the <see cref="IEventTypes" />.
        /// </summary>
        public IEventTypes EventTypes { get; }

        /// <summary>
        /// Gets the <see cref="IEventStore" />.
        /// </summary>
        public IEventStore EventStore { get; }

        /// <summary>
        /// Create a client builder for a Miroservice.
        /// </summary>
        /// <param name="microserviceId">The unique identifier for the microservice.</param>
        /// <returns>The <see cref="ClientBuilder"/> to build the <see cref="Client"/> from.</returns>
        public static ClientBuilder ForMicroservice(MicroserviceId microserviceId)
            => new ClientBuilder(microserviceId, Version.NotSet, Environment.Undetermined);

        /// <summary>
        /// Create a client builder for a Miroservice.
        /// </summary>
        /// /// <param name="microserviceId">The unique identifier for the microservice.</param>
        /// <param name="version">Version of the microservice.</param>
        /// <returns>The <see cref="ClientBuilder"/> to build the <see cref="Client"/> from.</returns>
        public static ClientBuilder ForMicroservice(MicroserviceId microserviceId, Version version)
            => new ClientBuilder(microserviceId, version, Environment.Undetermined);

        /// <summary>
        /// Create a client builder for a Microservice.
        /// </summary>
        /// <param name="microserviceId">The unique identifier for the microservice.</param>
        /// <param name="version">Version of the microservice.</param>
        /// <param name="environment">The environment the software is running in.</param>
        /// <returns>The <see cref="ClientBuilder"/> to build the <see cref="Client"/> from.</returns>
        public static ClientBuilder ForMicroservice(MicroserviceId microserviceId, Version version, Environment environment)
            => new ClientBuilder(microserviceId, version, environment);
    }
}

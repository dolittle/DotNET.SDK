// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Filters;
using Dolittle.SDK.Events.Processing;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Microservices;
using Dolittle.SDK.Services;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK
{
    /// <summary>
    /// Represents a builder for building <see cref="Client" />.
    /// </summary>
    public class ClientBuilder
    {
        readonly EventTypesBuilder _eventTypesBuilder;
        readonly EventFiltersBuilder _eventFiltersBuilder;
        readonly MicroserviceId _microserviceId;
        string _host = "localhost";
        ushort _port = 50053;
        Microservices.Version _version;
        Microservices.Environment _environment;
        CancellationToken _cancellation;

        ILoggerFactory _loggerFactory = LoggerFactory.Create(_ =>
            {
                _.SetMinimumLevel(LogLevel.Trace);
                _.AddConsole();
            });

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientBuilder"/> class.
        /// </summary>
        /// <param name="microserviceId">The <see cref="MicroserviceId"/> of the microservice.</param>
        /// <param name="version">The <see cref="Microservices.Version"/> of the microservice.</param>
        /// <param name="environment">The <see cref="Microservices.Environment"/> of the microservice.</param>
        public ClientBuilder(MicroserviceId microserviceId, Microservices.Version version, Microservices.Environment environment)
        {
            _microserviceId = microserviceId;
            _version = version;
            _environment = environment;
            _cancellation = default;

            _eventTypesBuilder = new EventTypesBuilder(_loggerFactory);
            _eventFiltersBuilder = new EventFiltersBuilder();
        }

        /// <summary>
        /// Sets the version of the microservice.
        /// </summary>
        /// <param name="version">The version of the microservice.</param>
        /// <returns>The client builder for continuation.</returns>
        public ClientBuilder WithVersion(Microservices.Version version)
        {
            _version = version;
            return this;
        }

        /// <summary>
        /// Sets the environment in which the microservice is running.
        /// </summary>
        /// <param name="environment">The environment in which the microservice is running.</param>
        /// <returns>The client builder for continuation.</returns>
        public ClientBuilder WithEnvironment(Microservices.Environment environment)
        {
             _environment = environment;
             return this;
        }

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
        /// Sets the cancellation token for cancelling pending operations on the Runtime.
        /// </summary>
        /// <param name="cancellation">The cancellation token for cancelling pending operations on the Runtime.</param>
        /// <returns>The client builder for continuation.</returns>
        public ClientBuilder WithCancellationToken(CancellationToken cancellation)
        {
            _cancellation = cancellation;
            return this;
        }

        /// <summary>
        /// Connect to a specific host and port for the Dolittle runtime.
        /// </summary>
        /// <param name="host">The host name to connect to.</param>
        /// <param name="port">The port to connect to.</param>
        /// <returns>The client builder for continuation.</returns>
        /// <remarks>If not used, the default host of 'localhost' and port 50053 will be used.</remarks>
        public ClientBuilder ConnectToRuntime(string host, uint port)
        {
            _host = host;
            _port = port;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="ILoggerFactory"/> for the client.
        /// </summary>
        /// <param name="factory">The given <see cref="ILoggerFactory"/>.</param>
        /// <returns>The client builder for continuation.</returns>
        /// <remarks>If not used, a factory with 'Trace' level logging will be used.</remarks>
        public ClientBuilder ConfigureLogging(ILoggerFactory factory)
        {
            _loggerFactory = factory;
            return this;
        }

        /// <summary>
        /// Build the Client.
        /// </summary>
        /// <returns>The <see cref="Client"/>.</returns>
        public Client Build()
        {
            var executionContextManager = new ExecutionContextManager(_microserviceId, _version, _environment, _loggerFactory.CreateLogger<ExecutionContextManager>());
            var eventTypes = _eventTypesBuilder.Build();
            var methodCaller = new MethodCaller(_host, _port);
            var reverseCallClientsCreator = new ReverseCallClientCreator(
                TimeSpan.FromSeconds(5),
                methodCaller,
                executionContextManager,
                _loggerFactory);
            var eventProcessingRequestConverter = new EventProcessingRequestConverter(eventTypes);
            var eventProcessors = new EventProcessors(reverseCallClientsCreator, _loggerFactory.CreateLogger<EventProcessors>());
            _eventFiltersBuilder.BuildAndRegister(eventProcessors, eventProcessingRequestConverter, _loggerFactory, _cancellation);

            var eventConverter = new EventConverter(eventTypes);
            var eventStore = new EventStore(methodCaller, eventConverter, executionContextManager, eventTypes, _loggerFactory.CreateLogger<EventStore>());
            return new Client(_loggerFactory.CreateLogger<Client>(), executionContextManager, eventTypes, eventStore);
        }
    }
}

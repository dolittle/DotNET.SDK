// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Concepts;
using Dolittle.SDK.Events.Processing.Internal;
using Dolittle.SDK.Services;
using Google.Protobuf;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Processing
{
    /// <summary>
    /// An implementation of <see cref="IEventProcessors" />.
    /// </summary>
    public class EventProcessors : IEventProcessors
    {
        readonly ICreateReverseCallClients _reverseCallClientsCreator;
        readonly ICoordinateProcessing _processingCoordinator;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventProcessors"/> class.
        /// </summary>
        /// <param name="reverseCallClientsCreator">The <see cref="ICreateReverseCallClients" />.</param>
        /// <param name="processingCoordinator">The <see cref="ICoordinateProcessing" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EventProcessors(
            ICreateReverseCallClients reverseCallClientsCreator,
            ICoordinateProcessing processingCoordinator,
            ILogger logger)
        {
            _reverseCallClientsCreator = reverseCallClientsCreator;
            _processingCoordinator = processingCoordinator;
            _logger = logger;
        }

        /// <inheritdoc/>
        public void Register<TIdentifier, TClientMessage, TServerMessage, TRegisterArguments, TRegisterResponse, TRequest, TResponse>(
            IEventProcessor<TIdentifier, TRegisterArguments, TRequest, TResponse> eventProcessor,
            IAmAReverseCallProtocol<TClientMessage, TServerMessage, TRegisterArguments, TRegisterResponse, TRequest, TResponse> protocol,
            CancellationToken cancellationToken)
            where TIdentifier : ConceptAs<Guid>
            where TClientMessage : class, IMessage
            where TServerMessage : class, IMessage
            where TRegisterArguments : class
            where TRegisterResponse : class
            where TRequest : class
            where TResponse : class
        {
            var processor = Task.Run(() => RunProcessorForeverUntilCancelled(eventProcessor, protocol, cancellationToken), cancellationToken);
            _processingCoordinator.RegisterProcessor(processor);
        }

        async Task RunProcessorForeverUntilCancelled<TIdentifier, TClientMessage, TServerMessage, TRegisterArguments, TRegisterResponse, TRequest, TResponse>(
            IEventProcessor<TIdentifier, TRegisterArguments, TRequest, TResponse> eventProcessor,
            IAmAReverseCallProtocol<TClientMessage, TServerMessage, TRegisterArguments, TRegisterResponse, TRequest, TResponse> protocol,
            CancellationToken cancellationToken)
            where TIdentifier : ConceptAs<Guid>
            where TClientMessage : class, IMessage
            where TServerMessage : class, IMessage
            where TRegisterArguments : class
            where TRegisterResponse : class
            where TRequest : class
            where TResponse : class
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var client = _reverseCallClientsCreator.Create(protocol);

                    var connected = await client.Connect(eventProcessor.RegistrationRequest, cancellationToken).ConfigureAwait(false);
                    if (!connected)
                    {
                        _logger.LogWarning("{Kind} {Identifier} failed to connect to the Runtime, retrying in 1s", eventProcessor.Kind, eventProcessor.Identifier);
                        await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
                        continue;
                    }

                    var connectFailure = protocol.GetFailureFromConnectResponse(client.ConnectResponse);
                    if (connectFailure != null)
                    {
                        _logger.LogWarning("{Kind} {Identifier} received a failure from the Runtime, retrying in 1s. {FailureReason}", eventProcessor.Kind, eventProcessor.Identifier, connectFailure.Reason);
                        await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
                        continue;
                    }

                    _logger.LogDebug("{Kind} {Identifier} registered with the Runtime, start handling requests.", eventProcessor.Kind, eventProcessor.Identifier);
                    await client.Handle(eventProcessor, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    _logger.LogWarning(exception, "{Kind} {Identifier} registration failed with exception, retrying in 1s", eventProcessor.Kind, eventProcessor.Identifier);
                    await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
                }
            }

            _logger.LogDebug("{Kind} {identifier} handling of requests stopped.", eventProcessor.Kind, eventProcessor.Identifier);
        }
    }
}

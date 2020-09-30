// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
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
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventProcessors"/> class.
        /// </summary>
        /// <param name="reverseCallClientsCreator">The <see cref="ICreateReverseCallClients" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EventProcessors(
            ICreateReverseCallClients reverseCallClientsCreator,
            ILogger logger)
        {
            _reverseCallClientsCreator = reverseCallClientsCreator;
            _logger = logger;
        }

        /// <inheritdoc/>
        public void Register<TIdentifier, TClientMessage, TServerMessage, TRegisterArguments, TRegisterResponse, TRequest, TResponse>(IEventProcessor<TIdentifier, TRegisterArguments, TRequest, TResponse> eventProcessor, IAmAReverseCallProtocol<TClientMessage, TServerMessage, TRegisterArguments, TRegisterResponse, TRequest, TResponse> protocol, CancellationToken cancellationToken)
            where TIdentifier : ConceptAs<Guid>
            where TClientMessage : class, IMessage
            where TServerMessage : class, IMessage
            where TRegisterArguments : class
            where TRegisterResponse : class
            where TRequest : class
            where TResponse : class
        {
            var client = _reverseCallClientsCreator.Create(eventProcessor.RegistrationRequest, eventProcessor, protocol);
            client.Subscribe(
                _ => EventProcessorRegistered(eventProcessor),
                error => EventProcessorRegistrationFailed(error, eventProcessor),
                () => EventProcessorRegistrationStopped(eventProcessor),
                cancellationToken);
        }

        void EventProcessorRegistered<TIdentifier, TRegisterArguments, TRequest, TResponse>(IEventProcessor<TIdentifier, TRegisterArguments, TRequest, TResponse> eventProcessor)
            where TIdentifier : ConceptAs<Guid>
            where TRegisterArguments : class
            where TRequest : class
            where TResponse : class
        {
            _logger.LogDebug("{Kind} {Identifier} registered with the Runtime, start handling requests.", eventProcessor.Kind, eventProcessor.Identifier);
        }

        void EventProcessorRegistrationFailed<TIdentifier, TRegisterArguments, TRequest, TResponse>(Exception exception, IEventProcessor<TIdentifier, TRegisterArguments, TRequest, TResponse> eventProcessor)
            where TIdentifier : ConceptAs<Guid>
            where TRegisterArguments : class
            where TRequest : class
            where TResponse : class
        {
            _logger.LogError(exception, "{Kind} {Identifier} registration failed.", eventProcessor.Kind, eventProcessor.Identifier);
        }

        void EventProcessorRegistrationStopped<TIdentifier, TRegisterArguments, TRequest, TResponse>(IEventProcessor<TIdentifier, TRegisterArguments, TRequest, TResponse> eventProcessor)
            where TIdentifier : ConceptAs<Guid>
            where TRegisterArguments : class
            where TRequest : class
            where TResponse : class
        {
            _logger.LogDebug("Registering {Kind} {identifier} handling of requests stopped.", eventProcessor.Kind, eventProcessor.Identifier);
        }
    }
}

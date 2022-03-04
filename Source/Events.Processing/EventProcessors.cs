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

namespace Dolittle.SDK.Events.Processing;

/// <summary>
/// An implementation of <see cref="IEventProcessors" />.
/// </summary>
public class EventProcessors : IEventProcessors
{
    readonly ICreateReverseCallClients _reverseCallClientsCreator;
    readonly ICoordinateProcessing _processingCoordinator;
    readonly ILogger _logger;
#if NETCOREAPP3_1
    ushort _eventProcessoCounter = 0;
#endif
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
#if NETCOREAPP3_1
        if (++_eventProcessoCounter == 100)
        {
            _logger.LogWarning("There are more than 100 event processors registered, this might cause your application to hang when using netcoreapp 3.1. Please reduce the amount of event processors or upgrade your app to use net5 or later version of .NET");
        }
#endif
        var processor = Task.Run(() => RunProcessorForeverUntilCancelled(eventProcessor, protocol, cancellationToken));
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
                    _logger.FailedToConnectToRuntime(eventProcessor.Kind, eventProcessor.Identifier);
                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken).ConfigureAwait(false);
                    continue;
                }

                var connectFailure = protocol.GetFailureFromConnectResponse(client.ConnectResponse);
                if (connectFailure != null)
                {
                    _logger.ReceivedFailureFromRuntime(eventProcessor.Kind, eventProcessor.Identifier, connectFailure.Reason);
                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken).ConfigureAwait(false);
                    continue;
                }

                _logger.Registered(eventProcessor.Kind, eventProcessor.Identifier);
                await client.Handle(eventProcessor, cancellationToken).ConfigureAwait(false);
                
                if (!cancellationToken.IsCancellationRequested)
                {
                    _logger.StoppedDuringProcessing(eventProcessor.Kind, eventProcessor.Identifier);
                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken).ConfigureAwait(false);
                }
            }
            catch (Exception exception) when (!cancellationToken.IsCancellationRequested)
            {
                _logger.RegistrationFailed(eventProcessor.Kind, eventProcessor.Identifier, exception);
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken).ConfigureAwait(false);
            }
        }

        _logger.ProcessingCompleted(eventProcessor.Kind, eventProcessor.Identifier);
    }
}

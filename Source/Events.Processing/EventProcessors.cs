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
    static readonly TimeSpan _gracePeriod = TimeSpan.FromSeconds(10);

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
        if (protocol.SupportsDisconnectMessages)
        {
            var processor = Task.Run(() => RunProcessorForeverUntilCancelledWithGracefulDisconnect(eventProcessor, protocol, cancellationToken),
                CancellationToken.None);
            _processingCoordinator.RegisterProcessor(processor);
        }
        else
        {
            var processor = Task.Run(() => RunProcessorForeverUntilCancelled(eventProcessor, protocol, cancellationToken), CancellationToken.None);
            _processingCoordinator.RegisterProcessor(processor);
        }
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

    async Task RunProcessorForeverUntilCancelledWithGracefulDisconnect<TIdentifier, TClientMessage, TServerMessage, TRegisterArguments, TRegisterResponse,
        TRequest, TResponse>(
        IEventProcessor<TIdentifier, TRegisterArguments, TRequest, TResponse> eventProcessor,
        IAmAReverseCallProtocol<TClientMessage, TServerMessage, TRegisterArguments, TRegisterResponse, TRequest, TResponse> protocol,
        CancellationToken shutdownToken)
        where TIdentifier : ConceptAs<Guid>
        where TClientMessage : class, IMessage
        where TServerMessage : class, IMessage
        where TRegisterArguments : class
        where TRegisterResponse : class
        where TRequest : class
        where TResponse : class
    {
        using var connectionCancellation = new CancellationTokenSource();


        while (!shutdownToken.IsCancellationRequested)
        {
            try
            {
                var client = _reverseCallClientsCreator.Create(protocol);
                await using var disconnectCallback = shutdownToken.Register(async () =>
                {
                    var timeout = Task.Delay(_gracePeriod + TimeSpan.FromSeconds(1), CancellationToken.None);
                    var initiateDisconnectMessage = protocol.CreateInitiateDisconnectMessage(_gracePeriod);
                    if (initiateDisconnectMessage is not null) // Should never be null at this point
                    {
                        await client.WriteMessage(initiateDisconnectMessage, CancellationToken.None);
                    }

                    try
                    {
                        await timeout;
                        // If the disconnect ack has not been received, we will forcefully close the connection
                        // ReSharper disable once AccessToDisposedClosure
                        connectionCancellation.Cancel();
                    }
                    catch
                    {
                        // Ignored. If the connection is already closed, cancelling will throw
                    }
                });


                var connected = await client.Connect(eventProcessor.RegistrationRequest, connectionCancellation.Token).ConfigureAwait(false);
                if (!connected)
                {
                    _logger.FailedToConnectToRuntime(eventProcessor.Kind, eventProcessor.Identifier);
                    await Task.Delay(TimeSpan.FromSeconds(1), connectionCancellation.Token).ConfigureAwait(false);
                    continue;
                }

                var connectFailure = protocol.GetFailureFromConnectResponse(client.ConnectResponse);
                if (connectFailure != null)
                {
                    _logger.ReceivedFailureFromRuntime(eventProcessor.Kind, eventProcessor.Identifier, connectFailure.Reason);
                    await Task.Delay(TimeSpan.FromSeconds(1), connectionCancellation.Token).ConfigureAwait(false);
                    continue;
                }

                _logger.Registered(eventProcessor.Kind, eventProcessor.Identifier);
                await client.Handle(eventProcessor, connectionCancellation.Token).ConfigureAwait(false);

                if (!shutdownToken.IsCancellationRequested)
                {
                    _logger.StoppedDuringProcessing(eventProcessor.Kind, eventProcessor.Identifier);
                    await Task.Delay(TimeSpan.FromSeconds(1), connectionCancellation.Token).ConfigureAwait(false);
                }
            }
            catch (Exception exception) when (!shutdownToken.IsCancellationRequested)
            {
                _logger.RegistrationFailed(eventProcessor.Kind, eventProcessor.Identifier, exception);
                await Task.Delay(TimeSpan.FromSeconds(1), connectionCancellation.Token).ConfigureAwait(false);
            }
        }

        _logger.ProcessingCompleted(eventProcessor.Kind, eventProcessor.Identifier);
    }
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Protobuf.Contracts;
using Dolittle.SDK.Concepts;
using Dolittle.SDK.Services;
using Google.Protobuf;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Processing.Internal
{
    /// <summary>
    /// An implementation of <see cref="IEventProcessorRegistration{TIdentifier, TRegisterArguments, TRegisterResponse}" />.
    /// </summary>
    /// <typeparam name="TIdentifier">A <see cref="Type" /> extending <see cref="ConceptAs{T}" /> <see cref="Guid" />.</typeparam>
    /// <typeparam name="TClientMessage">The <see cref="Type" /> of the client message .</typeparam>
    /// <typeparam name="TServerMessage">The <see cref="Type" /> of the server message .</typeparam>
    /// <typeparam name="TRegisterArguments">The <see cref="Type" /> of the registration arguments.</typeparam>
    /// <typeparam name="TRegisterResponse">The <see cref="Type" /> of the registration response.</typeparam>
    /// <typeparam name="TRequest">The <see cref="Type" /> of the request.</typeparam>
    /// <typeparam name="TResponse">The <see cref="Type" /> of the response.</typeparam>
    public abstract class EventProcessorRegistration<TIdentifier, TClientMessage, TServerMessage, TRegisterArguments, TRegisterResponse, TRequest, TResponse> : IEventProcessorRegistration<TIdentifier, TRegisterArguments, TRegisterResponse>
        where TIdentifier : ConceptAs<Guid>
        where TClientMessage : IMessage
        where TServerMessage : IMessage
        where TRegisterArguments : class
        where TRegisterResponse : class
        where TRequest : class
        where TResponse : class
    {
        readonly IReverseCallHandler<TRequest, TResponse> _reverseCallHandler;
        readonly ICreateReverseCallClients _reverseCallClientsCreator;
        readonly IAmAReverseCallProtocol<TClientMessage, TServerMessage, TRegisterArguments, TRegisterResponse, TRequest, TResponse> _protocol;
        readonly EventProcessorKind _eventProcessorKind;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventProcessorRegistration{TIdentifier, TClientMessage, TServerMessage, TRegisterArguments, TRegisterResponse, TRequest, TResponse}"/> class.
        /// </summary>
        /// <param name="reverseCallHandler">The <see cref="IReverseCallHandler{TRequest, TResponse}" />.</param>
        /// <param name="reverseCallClientsCreator">The <see cref="ICreateReverseCallClients" />.</param>
        /// <param name="protocol">The <see cref="IAmAReverseCallProtocol{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}" />.</param>
        /// <param name="eventProcessorId">The <typeparamref name="TIdentifier" /> unique identifier of the event processor.</param>
        /// <param name="eventProcessorKind">The <see cref="EventProcessorKind" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        protected EventProcessorRegistration(
            IReverseCallHandler<TRequest, TResponse> reverseCallHandler,
            ICreateReverseCallClients reverseCallClientsCreator,
            IAmAReverseCallProtocol<TClientMessage, TServerMessage, TRegisterArguments, TRegisterResponse, TRequest, TResponse> protocol,
            TIdentifier eventProcessorId,
            EventProcessorKind eventProcessorKind,
            ILogger logger)
        {
            _reverseCallHandler = reverseCallHandler;
            _reverseCallClientsCreator = reverseCallClientsCreator;
            _protocol = protocol;
            EventProcessorId = eventProcessorId;
            _eventProcessorKind = eventProcessorKind;
            _logger = logger;
        }

        /// <inheritdoc/>
        public TIdentifier EventProcessorId { get; }

        /// <summary>
        /// Gets the <typeparamref name="TRegisterArguments"/> for registering this event processor.
        /// </summary>
        protected abstract TRegisterArguments Arguments { get; }

        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<TRegisterResponse> observer)
        {
            _logger.LogDebug("Registering {Kind} {Identifier} with the Runtime", _eventProcessorKind, EventProcessorId);
            return _reverseCallClientsCreator.Create(Arguments, _reverseCallHandler, _protocol)
                        .Subscribe(
                            registerResponse => OnNext(registerResponse, observer),
                            error => OnError(error, observer),
                            () => OnCompleted(observer));
        }

        /// <summary>
        /// Gets a <see cref="Failure" /> from a <typeparamref name="TRegisterResponse"/>.
        /// </summary>
        /// /// <param name="response">The <typeparamref name="TRegisterResponse"/>.</param>
        /// <returns>The <see cref="Failure" /> from the <typeparamref name="TRegisterResponse"/> or null if there was no failure.</returns>
        protected abstract Failure GetFailureFromRegisterResponse(TRegisterResponse response);

        void OnNext(TRegisterResponse registerResponse, IObserver<TRegisterResponse> subscriber)
        {
            var failure = GetFailureFromRegisterResponse(registerResponse);
            if (failure != null)
            {
                subscriber.OnError(new RegistrationFailed(_eventProcessorKind, EventProcessorId, failure));
                return;
            }

            _logger.LogDebug("{Kind} {Identifier} registered with the Runtime, start handling requests.", _eventProcessorKind, EventProcessorId);
            subscriber.OnNext(registerResponse);
        }

        void OnError(Exception exception, IObserver<TRegisterResponse> subscriber) => subscriber.OnError(exception);

        void OnCompleted(IObserver<TRegisterResponse> subscriber)
        {
            _logger.LogDebug("Registering {Kind} {identifier} handling of requests completed.");
            subscriber.OnCompleted();
        }
    }
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Concepts;
using Dolittle.Logging;
using Dolittle.Protobuf.Contracts;
using Dolittle.Resilience;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Services;
using Dolittle.Services.Clients;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace Dolittle.Events.Processing.Internal
{
    /// <summary>
    /// Partial implementation of <see cref="IEventProcessor"/>.
    /// </summary>
    /// <typeparam name="TIdentifier">Type of the identifier for the implementation of the event processor.</typeparam>
    /// <typeparam name="TClientMessage">Type of the <see cref="IMessage">messages</see> that is sent from the client to the server.</typeparam>
    /// <typeparam name="TServerMessage">Type of the <see cref="IMessage">messages</see> that is sent from the server to the client.</typeparam>
    /// <typeparam name="TConnectArguments">Type of the arguments that are sent along with the initial Connect call.</typeparam>
    /// <typeparam name="TConnectResponse">Type of the response that is received after the initial Connect call.</typeparam>
    /// <typeparam name="TRequest">Type of the requests sent from the server to the client using <see cref="IReverseCallDispatcher{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}.Call"/>.</typeparam>
    /// <typeparam name="TResponse">Type of the responses received from the client using <see cref="IReverseCallDispatcher{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}.Call"/>.</typeparam>
    public abstract class EventProcessor<TIdentifier, TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> : IEventProcessor
        where TIdentifier : ConceptAs<Guid>
        where TClientMessage : IMessage, new()
        where TServerMessage : IMessage, new()
        where TConnectArguments : class
        where TConnectResponse : class
        where TRequest : class
        where TResponse : class
    {
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventProcessor{TIdentifier, TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to use for logging.</param>
        protected EventProcessor(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets a string representing the kind of event processor. This is used for logging and exception messages.
        /// </summary>
        protected abstract string Kind { get; }

        /// <summary>
        /// Gets a <typeparamref name="TIdentifier"/> that represents the identity of the event processor. This is used for logging and exception messages.
        /// </summary>
        protected abstract TIdentifier Identifier { get; }

        /// <inheritdoc/>
        public async Task RegisterAndHandle(CancellationToken cancellationToken)
        {
            _logger.Debug("Registering {Kind} {Id} with the Runtime.", Kind, Identifier);
            var client = CreateClient();
            var receivedResponse = await client.Connect(GetRegisterArguments(), cancellationToken).ConfigureAwait(false);
            if (cancellationToken.IsCancellationRequested) return;
            ThrowIfNotReceivedResponse(receivedResponse);
            ThrowIfRegisterFailure(GetFailureFromRegisterResponse(client.ConnectResponse));
            _logger.Trace("{Kind} {Id} registered with the Runtime, start handling requests.", Kind, Identifier);
            await client.Handle(CatchingHandle, cancellationToken).ConfigureAwait(false);
            _logger.Trace("{Kind} {Id} handling of requests completed.", Kind, Identifier);
        }

        /// <inheritdoc/>
        public Task RegisterAndHandleWithPolicy(IAsyncPolicy policy, CancellationToken cancellationToken)
            => policy.Execute(
                async (cancellationToken) =>
                {
                    try
                    {
                        await RegisterAndHandle(cancellationToken).ConfigureAwait(false);
                    }
                    catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        while (ex.InnerException != null) ex = ex.InnerException;
                        _logger.Warning(ex, "Failed to register {Kind} {Id} with the Runtime.", Kind, Identifier);
                        ExceptionDispatchInfo.Capture(ex).Throw();
                    }
                },
                cancellationToken);

        /// <inheritdoc/>
        public async Task RegisterAndHandleForeverWithPolicy(IAsyncPolicy policy, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await RegisterAndHandleWithPolicy(policy, cancellationToken).ConfigureAwait(false);
                await Task.Delay(1000).ConfigureAwait(false);
                _logger.Trace("Restaring {Kind} {Id}.", Kind, Identifier);
            }
        }

        /// <summary>
        /// The method that will be called to create an instance of a reverse call client for connecting to the Runtime.
        /// </summary>
        /// <returns>An <see cref="IReverseCallClient{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}"/> for connecting to the Runtime.</returns>
        protected abstract IReverseCallClient<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> CreateClient();

        /// <summary>
        /// The method that will be called to provide registration arguments to register the event processor with the Runtime.
        /// </summary>
        /// <returns><typeparamref name="TConnectArguments"/> to use for registering with the Runtime.</returns>
        protected abstract TConnectArguments GetRegisterArguments();

        /// <summary>
        /// The method that will be called to check if the registration with the Runtime failed.
        /// If a <see cref="Failure"/> was not present on the response (null) the registration request is considered successful.
        /// </summary>
        /// <param name="response">The <typeparamref name="TConnectResponse"/> recieved from the Runtime during registration.</param>
        /// <returns>An optional <see cref="Failure"/> present on the <typeparamref name="TConnectResponse"/>.</returns>
        protected abstract Failure GetFailureFromRegisterResponse(TConnectResponse response);

        /// <summary>
        /// The method that will be called to get the retry processing state from a request received from the Runtime.
        /// </summary>
        /// <param name="request">The <typeparamref name="TRequest"/> received from the Runtime.</param>
        /// <returns>The <see cref="RetryProcessingState"/> present on the <typeparamref name="TRequest"/>.</returns>
        protected abstract RetryProcessingState GetRetryProcessingState(TRequest request);

        /// <summary>
        /// The method that will be called to handle requests from the Runtime for this event processor.
        /// </summary>
        /// <param name="request">The <typeparamref name="TRequest"/> recieved from the Runtime.</param>
        /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
        /// <returns>A <see cref="Task{TResponse}"/> that, when resolved, returns the <typeparamref name="TResponse"/> to send back to the Runtime.</returns>
        protected abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// The method that will be called to create a response to the Runtime in the case when handling of a request fails.
        /// The provided failure will have the exception message and retry properties set based on the retry strategy.
        /// </summary>
        /// <param name="failure">A <see cref="ProcessorFailure"/> populated with the failure reason and retry strategy.</param>
        /// <returns>A <typeparamref name="TResponse"/> to send back to the Runtime.</returns>
        protected abstract TResponse CreateResponseFromFailure(ProcessorFailure failure);

        async Task<TResponse> CatchingHandle(TRequest request, CancellationToken cancellationToken)
        {
            RetryProcessingState retryState = null;
            try
            {
                retryState = GetRetryProcessingState(request);
                return await Handle(request, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) ex = ex.InnerException;

                var failure = new ProcessorFailure
                {
                    Reason = ex.Message,
                    Retry = true,
                    RetryTimeout = CalculateRetryTimeout(retryState),
                };

                _logger.Warning(ex, "Processing in {Kind} {Id} failed. Will retry in {RetryTimeout} seconds.", Kind, Identifier, failure.RetryTimeout.Seconds);

                return CreateResponseFromFailure(failure);
            }
        }

        Duration CalculateRetryTimeout(RetryProcessingState retryState)
        {
            var attempt = retryState?.RetryCount ?? 0 + 1;
            var seconds = Math.Min(attempt * 5, 60);
            return Duration.FromTimeSpan(TimeSpan.FromSeconds(seconds));
        }

        void ThrowIfNotReceivedResponse(bool receivedResponse)
        {
            if (!receivedResponse) throw new DidNotReceiveRegistrationResponse(Kind, Identifier);
        }

        void ThrowIfRegisterFailure(Failure registerFailure)
        {
            if (registerFailure != null) throw new RegistrationFailed(Kind, Identifier, registerFailure);
        }
    }
}
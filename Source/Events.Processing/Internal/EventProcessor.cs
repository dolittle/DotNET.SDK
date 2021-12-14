// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.SDK.Concepts;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.Events.Processing.Internal;

/// <summary>
/// Represents a base implementation of <see cref="IEventProcessor{TIdentifier, TRegisterResponse, TRequest, TResponse}" />.
/// </summary>
/// <typeparam name="TIdentifier">A <see cref="System.Type" /> extending <see cref="ConceptAs{T}" /> <see cref="Guid" />.</typeparam>
/// <typeparam name="TRegisterArguments">The <see cref="System.Type" /> of the registration arguments.</typeparam>
/// <typeparam name="TRequest">The <see cref="System.Type" /> of the request.</typeparam>
/// <typeparam name="TResponse">The <see cref="System.Type" /> of the response.</typeparam>
public abstract class EventProcessor<TIdentifier, TRegisterArguments, TRequest, TResponse> : IEventProcessor<TIdentifier, TRegisterArguments, TRequest, TResponse>
    where TIdentifier : ConceptAs<Guid>
    where TRegisterArguments : class
    where TRequest : class
    where TResponse : class
{
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventProcessor{TIdentifier, TRegisterArguments, TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="kind">The kind of the event processor.</param>
    /// <param name="identifier">The <typeparamref name="TIdentifier"/> identifier of the event processor.</param>
    /// <param name="logger">The <see cref="ILogger" />.</param>
    protected EventProcessor(
        EventProcessorKind kind,
        TIdentifier identifier,
        ILogger logger)
    {
        Kind = kind;
        Identifier = identifier;
        _logger = logger;
    }

    /// <inheritdoc/>
    public EventProcessorKind Kind { get; }

    /// <inheritdoc/>
    public TIdentifier Identifier { get; }

    /// <inheritdoc/>
    public abstract TRegisterArguments RegistrationRequest { get; }

    /// <inheritdoc/>
    public async Task<TResponse> Handle(TRequest request, ExecutionContext executionContext, CancellationToken cancellation)
    {
        RetryProcessingState retryProcessingState = null;
        try
        {
            retryProcessingState = GetRetryProcessingStateFromRequest(request);
            return await Process(request, executionContext, cancellation).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            var retryAttempt = retryProcessingState != default ? retryProcessingState.RetryCount : 1;
            var retrySeconds = Math.Min(5 * retryAttempt, 60);
            var retryTimeout = new Duration
            {
                Seconds = retrySeconds
            };
            var failure = new ProcessorFailure
            {
                Reason = ex.Message,
                Retry = true,
                RetryTimeout = retryTimeout
            };

            _logger.LogWarning(ex, "Processing in {Kind} {Identifier} failed. Will retry in {RetrySeconds}", Kind, Identifier, retrySeconds);

            return CreateResponseFromFailure(failure);
        }
    }

    /// <summary>
    /// Method that will be called to process an event processing request from the server.
    /// </summary>
    /// <param name="request">The <typeparamref name="TRequest"/> to process.</param>
    /// <param name="executionContext">The execution context to handle the request in.</param>
    /// <param name="cancellation">The <see cref="CancellationToken" /> used to cancel the processing of the request.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns a <typeparamref name="TResponse"/>.</returns>
    protected abstract Task<TResponse> Process(TRequest request, ExecutionContext executionContext, CancellationToken cancellation);

    /// <summary>
    /// Gets the <see cref="RetryProcessingState" /> from a <typeparamref name="TRequest"/>.
    /// </summary>
    /// <param name="request">The <typeparamref name="TRequest" />.</param>
    /// <returns>The <see cref="RetryProcessingState" /> from the <typeparamref name="TRequest"/>.</returns>
    protected abstract RetryProcessingState GetRetryProcessingStateFromRequest(TRequest request);

    /// <summary>
    /// Creates a <typeparamref name="TResponse"/> from a <see cref="ProcessorFailure" />.
    /// </summary>
    /// <param name="failure">The <see cref="ProcessorFailure" />.</param>
    /// <returns>The <typeparamref name="TResponse"/>.</returns>
    protected abstract TResponse CreateResponseFromFailure(ProcessorFailure failure);
}

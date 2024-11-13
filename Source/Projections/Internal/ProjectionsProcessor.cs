// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Processing;
using Dolittle.SDK.Events.Processing.Internal;
using Dolittle.SDK.Events.Store;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Projections.Actors;
using Dolittle.SDK.Protobuf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.Projections.Internal;

/// <summary>
/// Represents a <see cref="EventProcessor{TIdentifier, TRegisterArguments, TRequest, TResponse}" /> that can handle projections.
/// </summary>
/// <typeparam name="TReadModel">The type of the read model.</typeparam>
public class ProjectionsProcessor<TReadModel> : EventProcessor<ProjectionId, EventHandlerRegistrationRequest, HandleEventRequest, EventHandlerResponse>
    where TReadModel : ReadModel, new()
{
    readonly IProjection<TReadModel> _projection;
    readonly IEventProcessingConverter _eventConverter;
    readonly ILogger _logger;
    readonly string _activityName;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionsProcessor{TReadModel}"/> class.
    /// </summary>
    /// <param name="projection">The <see cref="IProjection{TReadModel}" />.</param>
    /// <param name="eventConverter">The <see cref="IEventProcessingConverter" />.</param>
    /// <param name="projectionConverter">The <see cref="IConvertProjectionsToSDK" />.</param>
    /// <param name="logger">The <see cref="ILogger" />.</param>
    public ProjectionsProcessor(
        IProjection<TReadModel> projection,
        IEventProcessingConverter eventConverter,
        ILogger logger)
        : base("Projection", projection.Identifier, logger)
    {
        _projection = projection;
        _eventConverter = eventConverter;
        _logger = logger;
        _activityName = "Projection " + projection.Alias?.Value ?? typeof(TReadModel).Name;
    }

    /// <inheritdoc/>
    public override EventHandlerRegistrationRequest RegistrationRequest
    {
        get
        {
            var registrationRequest = new EventHandlerRegistrationRequest
            {
                EventHandlerId = _projection.Identifier.ToProtobuf(),
                ScopeId = _projection.ScopeId.ToProtobuf(),
                Concurrency = 5
            };
            if (_projection.HasAlias)
            {
                registrationRequest.Alias = _projection.Alias.Value;
            }

            registrationRequest.EventTypes.AddRange(_projection.Events.Keys.Select(eventType => eventType.ToProtobuf()));

            return registrationRequest;
        }
    }

    /// <inheritdoc/>
    protected override async Task<EventHandlerResponse> Process(HandleEventRequest request, ExecutionContext executionContext, IServiceProvider serviceProvider,
        CancellationToken cancellation)
    {
        var committedEvent = _eventConverter.ToSDK(request.Event).Event;
        var eventContext = committedEvent.GetEventContext(executionContext);
        using var activity = eventContext.CommittedExecutionContext.StartChildActivity(_activityName + committedEvent.Content.GetType().Name)
            ?.Tag(committedEvent.EventType);
        try
        {
            var client = serviceProvider.GetRequiredService<IProjectionClient<TReadModel>>();
            var result = await client.On(committedEvent.Content, committedEvent.EventType, eventContext, cancellation);
            activity?.SetTag("result", result.ToString());
            return new EventHandlerResponse();
        }
        catch (Exception e)
        {
            _logger.ErrorProcessingProjectionEvent(e, committedEvent);
            activity?.AddException(e);
            throw;
        }
    }

    /// <inheritdoc/>
    protected override RetryProcessingState GetRetryProcessingStateFromRequest(HandleEventRequest request)
        => request.RetryProcessingState;

    /// <inheritdoc/>
    protected override EventHandlerResponse CreateResponseFromFailure(ProcessorFailure failure)
        => new() { Failure = failure };
}

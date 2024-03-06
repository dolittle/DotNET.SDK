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
using OpenTelemetry.Trace;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.Projections.Internal;

/// <summary>
/// Represents a <see cref="EventProcessor{TIdentifier, TRegisterArguments, TRequest, TResponse}" /> that can handle projections.
/// </summary>
/// <typeparam name="TReadModel">The type of the read model.</typeparam>
public class ProjectionsProcessor<TReadModel> : EventProcessor<ProjectionId, EventHandlerRegistrationRequest, HandleEventRequest, EventHandlerResponse>
    where TReadModel : ProjectionBase, new()
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
            _logger.LogError(e, "Error processing projection event {Event} for ", committedEvent);
            activity?.RecordException(e);
            throw;
        }
    }

    /// <inheritdoc/>
    protected override RetryProcessingState GetRetryProcessingStateFromRequest(HandleEventRequest request)
        => request.RetryProcessingState;

    /// <inheritdoc/>
    protected override EventHandlerResponse CreateResponseFromFailure(ProcessorFailure failure)
        => new() { Failure = failure };

    static ProjectionEventSelector CreateProjectionEventSelector(EventSelector eventSelector)
    {
        static ProjectionEventSelector WithEventType(EventSelector eventSelector, Action<ProjectionEventSelector> callback)
        {
            var message = new ProjectionEventSelector();
            callback(message);
            message.EventType = eventSelector.EventType.ToProtobuf();
            return message;
        }

        return eventSelector.KeySelector.Type switch
        {
            KeySelectorType.EventSourceId => WithEventType(eventSelector, _ => _.EventSourceKeySelector = new EventSourceIdKeySelector()),
            KeySelectorType.PartitionId => WithEventType(eventSelector, _ => _.PartitionKeySelector = new PartitionIdKeySelector()),
            KeySelectorType.Property => WithEventType(eventSelector,
                _ => _.EventPropertyKeySelector = new EventPropertyKeySelector { PropertyName = eventSelector.KeySelector.Expression ?? string.Empty }),
            KeySelectorType.Static => WithEventType(eventSelector,
                _ => _.StaticKeySelector = new StaticKeySelector { StaticKey = eventSelector.KeySelector.StaticKey ?? string.Empty }),
            KeySelectorType.EventOccurred => WithEventType(eventSelector,
                _ => _.EventOccurredKeySelector = new EventOccurredKeySelector { Format = eventSelector.KeySelector.OccurredFormat ?? string.Empty }),
            _ => throw new UnknownKeySelectorType(eventSelector.KeySelector.Type)
        };
    }
}

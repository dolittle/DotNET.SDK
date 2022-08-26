// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Contracts;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Projections.Contracts;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Processing.Internal;
using Dolittle.SDK.Events.Store;
using Dolittle.SDK.Events.Store.Converters;
using Dolittle.SDK.Projections;
using Dolittle.SDK.Projections.Store;
using Dolittle.SDK.Projections.Store.Converters;
using Dolittle.SDK.Protobuf;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.Embeddings.Internal;

/// <summary>
/// Represents a <see cref="EventProcessor{TIdentifier, TRegisterArguments, TRequest, TResponse}" /> that can handle projections.
/// </summary>
/// <typeparam name="TReadModel">The type of the read model.</typeparam>
public class EmbeddingsProcessor<TReadModel> : EventProcessor<EmbeddingId, EmbeddingRegistrationRequest, EmbeddingRequest, EmbeddingResponse>
    where TReadModel : class, new()
{
    readonly IEmbedding<TReadModel> _embedding;
    readonly IConvertEventsToProtobuf _eventsToProtobuf;
    readonly IConvertProjectionsToSDK _projectionConverter;
    readonly IEventTypes _eventTypes;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbeddingsProcessor{TReadModel}"/> class.
    /// </summary>
    /// <param name="embedding">The <see cref="IEmbedding{TReadModel}" />.</param>
    /// <param name="eventsToProtobuf">The <see cref="IConvertEventsToProtobuf" />.</param>
    /// <param name="projectionConverter">The <see cref="IConvertProjectionsToSDK" />.</param>
    /// <param name="eventTypes">The event types.</param>
    /// <param name="logger">The <see cref="ILogger" />.</param>
    public EmbeddingsProcessor(
        IEmbedding<TReadModel> embedding,
        IConvertEventsToProtobuf eventsToProtobuf,
        IConvertProjectionsToSDK projectionConverter,
        IEventTypes eventTypes,
        ILogger logger)
        : base("Embedding", embedding.Identifier.Id, logger)
    {
        _embedding = embedding;
        _eventsToProtobuf = eventsToProtobuf;
        _projectionConverter = projectionConverter;
        _eventTypes = eventTypes;
    }

    /// <inheritdoc/>
    public override EmbeddingRegistrationRequest RegistrationRequest
    {
        get
        {
            var registrationRequest = new EmbeddingRegistrationRequest
            {
                EmbeddingId = _embedding.Identifier.Id.ToProtobuf(),
                InitialState = JsonConvert.SerializeObject(_embedding.InitialState, Formatting.None),
            };
            registrationRequest.Events.AddRange(_embedding.Events.Select(_ => _.ToProtobuf()));
            return registrationRequest;
        }
    }

    /// <inheritdoc/>
    protected override Task<EmbeddingResponse> Process(EmbeddingRequest request, ExecutionContext executionContext, IServiceProvider serviceProvider, CancellationToken cancellation)
        => request switch
        {
            { RequestCase: EmbeddingRequest.RequestOneofCase.Compare } => Task.FromResult(HandleCompareRequest(request.Compare, executionContext, cancellation)),
            { RequestCase: EmbeddingRequest.RequestOneofCase.Delete } => Task.FromResult(HandleDeleteRequest(request.Delete, executionContext, cancellation)),
            { RequestCase: EmbeddingRequest.RequestOneofCase.Projection } => HandleProjectionRequest(request.Projection, executionContext, cancellation),
            _ => throw new MissingEmbeddingInformation("Request")
        };

    /// <inheritdoc/>
    protected override RetryProcessingState GetRetryProcessingStateFromRequest(EmbeddingRequest request)
        => null;

    /// <inheritdoc/>
    protected override EmbeddingResponse CreateResponseFromFailure(ProcessorFailure failure)
        => new() { ProcessorFailure = failure };

    EmbeddingResponse HandleCompareRequest(EmbeddingCompareRequest request, ExecutionContext executionContext, CancellationToken cancellation)
        => HandleDeleteOrCompare(
            false,
            request.ProjectionState,
            executionContext,
            (currentState, context) => _embedding.Update(CreateReceivedState(request.EntityState), currentState.State, context, cancellation),
            events =>
            {
                var response = new EmbeddingResponse { Compare = new EmbeddingCompareResponse() };
                response.Compare.Events.AddRange(events);
                return response;
            });

    EmbeddingResponse HandleDeleteRequest(EmbeddingDeleteRequest request, ExecutionContext executionContext, CancellationToken cancellation)
        => HandleDeleteOrCompare(
            true,
            request.ProjectionState,
            executionContext,
            (currentState, context) => _embedding.Delete(currentState.State, context, cancellation),
            events =>
            {
                var response = new EmbeddingResponse { Delete = new EmbeddingDeleteResponse() };
                response.Delete.Events.AddRange(events);
                return response;
            });

    EmbeddingResponse HandleDeleteOrCompare(
        bool isDelete,
        ProjectionCurrentState protobufCurrentState,
        ExecutionContext executionContext,
        Func<CurrentState<TReadModel>, EmbeddingContext, UncommittedEvents> getEventsToCommit,
        Func<IReadOnlyList<Runtime.Events.Contracts.UncommittedEvent>, EmbeddingResponse> createResponse)
    {
        if (!_projectionConverter.TryConvert<TReadModel>(protobufCurrentState, out var currentState, out var error))
        {
            throw error;
        }

        var context = new EmbeddingContext(currentState.WasCreatedFromInitialState, currentState.Key, isDelete, executionContext);
        var result = getEventsToCommit(currentState, context);
        if (!_eventsToProtobuf.TryConvert(result, out var protobufEvents, out error))
        {
            throw error;
        }

        return createResponse(protobufEvents);
    }

    async Task<EmbeddingResponse> HandleProjectionRequest(EmbeddingProjectRequest request, ExecutionContext executionContext, CancellationToken cancellation)
    {
        if (!_projectionConverter.TryConvert<TReadModel>(request.CurrentState, out var currentState, out var error))
        {
            throw error;
        }

        var projectionContext = new EmbeddingProjectContext(currentState.WasCreatedFromInitialState, currentState.Key, request.Event.EventSourceId, executionContext);
        var eventType = request.Event.EventType.To<EventType, EventTypeId>();
        var content = DeserializeUncommittedEvent(eventType, request.Event.Content);
        var result = await _embedding.On(
            currentState.State,
            content,
            eventType,
            projectionContext,
            cancellation).ConfigureAwait(false);

        return result.Type switch
        {
            ProjectionResultType.Replace => new EmbeddingResponse { ProjectionReplace = new ProjectionReplaceResponse { State = JsonConvert.SerializeObject(result.UpdatedReadModel, Formatting.None) } },
            ProjectionResultType.Delete => new EmbeddingResponse { ProjectionDelete = new ProjectionDeleteResponse() },
            _ => throw new UnknownProjectionResultType(result.Type)
        };
    }

    object DeserializeUncommittedEvent(EventType eventType, string json)
    {
        if (!_eventTypes.HasTypeFor(eventType))
        {
            return JsonConvert.DeserializeObject(json);
        }
        var type = _eventTypes.GetTypeFor(eventType);
        return JsonConvert.DeserializeObject(json, type);

    }

    static TReadModel CreateReceivedState(string state)
        => JsonConvert.DeserializeObject<TReadModel>(state);
}

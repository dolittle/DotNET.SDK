// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Contracts;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Projections.Contracts;
using Dolittle.SDK.Events.Processing;
using Dolittle.SDK.Events.Processing.Internal;
using Dolittle.SDK.Events.Store;
using Dolittle.SDK.Projections.Store.Converters;
using Dolittle.SDK.Protobuf;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.Embeddings.Internal
{
    /// <summary>
    /// Represents a <see cref="EventProcessor{TIdentifier, TRegisterArguments, TRequest, TResponse}" /> that can handle projections.
    /// </summary>
    /// <typeparam name="TReadModel">The type of the read model.</typeparam>
    public class EmbeddingsProcessor<TReadModel> : EventProcessor<EmbeddingId, EmbeddingRegistrationRequest, EmbeddingRequest, EmbeddingResponse>
        where TReadModel : class, new()
    {
        readonly IEmbedding<TReadModel> _embedding;
        readonly IEventProcessingConverter _eventConverter;
        readonly IConvertProjectionsToSDK _projectionConverter;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddingsProcessor{TReadModel}"/> class.
        /// </summary>
        /// <param name="embedding">The <see cref="IEmbedding{TReadModel}" />.</param>
        /// <param name="eventConverter">The <see cref="IEventProcessingConverter" />.</param>
        /// <param name="projectionConverter">The <see cref="IConvertProjectionsToSDK" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EmbeddingsProcessor(
            IEmbedding<TReadModel> embedding,
            IEventProcessingConverter eventConverter,
            IConvertProjectionsToSDK projectionConverter,
            ILogger logger)
            : base("Embedding", embedding.Identifier, logger)
        {
            _embedding = embedding;
            _eventConverter = eventConverter;
            _projectionConverter = projectionConverter;
        }

        /// <inheritdoc/>
        public override EmbeddingRegistrationRequest RegistrationRequest
        {
            get
            {
                var registrationRequest = new EmbeddingRegistrationRequest
                {
                    EmbeddingId = _embedding.Identifier.ToProtobuf(),
                    InitialState = JsonConvert.SerializeObject(_embedding.InitialState, Formatting.None),

                };
                registrationRequest.Events.AddRange(_embedding.Events.Select(_ => _.ToProtobuf()));
                return registrationRequest;
            }
        }

        /// <inheritdoc/>
        protected override Task<EmbeddingResponse> Process(EmbeddingRequest request, ExecutionContext executionContext, CancellationToken cancellation)
            => request switch
            {
                { RequestCase: EmbeddingRequest.RequestOneofCase.Compare => }
            };

        async Task<EmbeddingResponse> HandleCompareRequest(EmbeddingCompareRequest request)
        {
            _projectionConverter.TryConvert<TReadModel>(request.ProjectionState, out var currentState, out var error)
            {
                throw error;
            }
        }

        // if (!_projectionConverter.TryConvert<TReadModel>(GetProjectionCurrentState(request), out var currentState, out var error))
        // {
        //     throw error;
        // }



        // var committedEvent = _eventConverter.ToSDK(request.Event).Event;
        // var eventContext = committedEvent.GetEventContext(executionContext);
        // var projectionContext = new ProjectionContext(currentState.WasCreatedFromInitialState, currentState.Key, eventContext);

        // var result = await _embedding
        //     .On(
        //         currentState.State,
        //         committedEvent.Content,
        //         committedEvent.EventType,
        //         projectionContext,
        //         cancellation)
        //     .ConfigureAwait(false);

        // return result.Type switch
        // {
        //     EmbeddingResultType.Replace => new ProjectionResponse { Replace = new ProjectionReplaceResponse { State = JsonConvert.SerializeObject(result.UpdatedReadModel, Formatting.None) } },
        //     EmbeddingResultType.Delete => new ProjectionResponse { Delete = new ProjectionDeleteResponse() },
        //     _ => throw new UnknownEmbeddingResultType(result.Type)
        // };
    }

    /// <inheritdoc/>
    protected override RetryProcessingState GetRetryProcessingStateFromRequest(EmbeddingRequest request)
        => null;

    /// <inheritdoc/>
    protected override EmbeddingResponse CreateResponseFromFailure(ProcessorFailure failure)
        => new EmbeddingResponse { ProcessorFailure = failure };

    ProjectionCurrentState GetProjectionCurrentState(EmbeddingRequest request)
    {
        return request switch
        {
            { RequestCase: EmbeddingRequest.RequestOneofCase.Compare } => request.Compare.ProjectionState,
            { RequestCase: EmbeddingRequest.RequestOneofCase.Delete } => request.Delete.ProjectionState,
            { RequestCase: EmbeddingRequest.RequestOneofCase.Projection } => request.Delete.ProjectionState,
            _ => throw new MissingEmbeddingInformation("No compare, delete or projection request in EmbeddingRequest")
        };
    }
}
}

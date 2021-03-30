// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.SDK.Events.Processing;
using Dolittle.SDK.Events.Processing.Internal;
using Dolittle.SDK.Events.Store;
using Dolittle.SDK.Protobuf;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.Projections.Internal
{
    /// <summary>
    /// Represents a <see cref="EventProcessor{TIdentifier, TRegisterArguments, TRequest, TResponse}" /> that can handle projections.
    /// </summary>
    /// <typeparam name="TReadModel">The type of the read model.</typeparam>
    public class ProjectionsProcessor<TReadModel> : EventProcessor<ProjectionId, ProjectionRegistrationRequest, ProjectionRequest, ProjectionResponse>
        where TReadModel : class, new()
    {
        readonly IProjection<TReadModel> _projection;
        readonly IEventProcessingConverter _converter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionsProcessor{TReadModel}"/> class.
        /// </summary>
        /// <param name="projection">The <see cref="IProjection{TReadModel}" />.</param>
        /// <param name="converter">The <see cref="IEventProcessingConverter" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public ProjectionsProcessor(
            IProjection<TReadModel> projection,
            IEventProcessingConverter converter,
            ILogger logger)
            : base("Projection", projection.Identifier, logger)
        {
            _projection = projection;
            _converter = converter;
        }

        /// <inheritdoc/>
        public override ProjectionRegistrationRequest RegistrationRequest
        {
            get
            {
                var registrationRequest = new ProjectionRegistrationRequest
                {
                    ProjectionId = _projection.Identifier.ToProtobuf(),
                    ScopeId = _projection.ScopeId.ToProtobuf(),
                    InitialState = JsonConvert.SerializeObject(_projection.InitialState, Formatting.None)
                };
                registrationRequest.Events.AddRange(_projection.Events.Select(_ =>
                {
                    return new ProjectionEventSelector
                    {
                        EventType = _.EventType.ToProtobuf(),
                        KeySelector = new ProjectionEventKeySelector
                        {
                            Type = _.KeySelector.Type.ToProtobuf(),
                            Expression = _.KeySelector.Expression
                        }
                    };
                }).ToArray());
                return registrationRequest;
            }
        }

        /// <inheritdoc/>
        protected override async Task<ProjectionResponse> Process(ProjectionRequest request, ExecutionContext executionContext, CancellationToken cancellation)
        {
            var committedEvent = _converter.ToSDK(request.Event).Event;
            var eventContext = committedEvent.GetEventContext(executionContext);
            var projectionContext = new ProjectionContext(request.CurrentState.Type == ProjectionCurrentStateType.CreatedFromInitialState, request.Key, eventContext);

            var result = await _projection
                .On(
                    JsonConvert.DeserializeObject<TReadModel>(request.CurrentState.State),
                    committedEvent.Content,
                    committedEvent.EventType,
                    projectionContext,
                    cancellation)
                .ConfigureAwait(false);

            var response = new ProjectionResponse
            {
                NextState = new ProjectionNextState { Type = result.Type.ToProtobuf() }
            };
            if (result.Type == ProjectionResultType.Replace) response.NextState.Value = JsonConvert.SerializeObject(result.UpdatedReadModel, Formatting.None);
            return response;
        }

        /// <inheritdoc/>
        protected override RetryProcessingState GetRetryProcessingStateFromRequest(ProjectionRequest request)
            => request.RetryProcessingState;

        /// <inheritdoc/>
        protected override ProjectionResponse CreateResponseFromFailure(ProcessorFailure failure)
            => new ProjectionResponse { Failure = failure };
    }
}

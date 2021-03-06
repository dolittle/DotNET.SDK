// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.SDK.Events.Processing;
using Dolittle.SDK.Events.Processing.Internal;
using Dolittle.SDK.Events.Store;
using Dolittle.SDK.Projections.Store.Converters;
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
        readonly IEventProcessingConverter _eventConverter;
        readonly IConvertProjectionsToSDK _projectionConverter;

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
            IConvertProjectionsToSDK projectionConverter,
            ILogger logger)
            : base("Projection", projection.Identifier, logger)
        {
            _projection = projection;
            _eventConverter = eventConverter;
            _projectionConverter = projectionConverter;
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
                registrationRequest.Events.AddRange(_projection.Events.Select(CreateProjectionEventSelector).ToArray());
                return registrationRequest;
            }
        }

        /// <inheritdoc/>
        protected override async Task<ProjectionResponse> Process(ProjectionRequest request, ExecutionContext executionContext, CancellationToken cancellation)
        {
            if (!_projectionConverter.TryConvert<TReadModel>(request.CurrentState, out var currentState, out var error))
            {
                throw error;
            }

            var committedEvent = _eventConverter.ToSDK(request.Event).Event;
            var eventContext = committedEvent.GetEventContext(executionContext);
            var projectionContext = new ProjectionContext(currentState.WasCreatedFromInitialState, currentState.Key, eventContext);

            var result = await _projection
                .On(
                    currentState.State,
                    committedEvent.Content,
                    committedEvent.EventType,
                    projectionContext,
                    cancellation)
                .ConfigureAwait(false);

            return result.Type switch
            {
                ProjectionResultType.Replace => new ProjectionResponse { Replace = new ProjectionReplaceResponse { State = JsonConvert.SerializeObject(result.UpdatedReadModel, Formatting.None) } },
                ProjectionResultType.Delete => new ProjectionResponse { Delete = new ProjectionDeleteResponse() },
                _ => throw new UnknownProjectionResultType(result.Type)
            };
        }

        /// <inheritdoc/>
        protected override RetryProcessingState GetRetryProcessingStateFromRequest(ProjectionRequest request)
            => request.RetryProcessingState;

        /// <inheritdoc/>
        protected override ProjectionResponse CreateResponseFromFailure(ProcessorFailure failure)
            => new ProjectionResponse { Failure = failure };

        ProjectionEventSelector CreateProjectionEventSelector(EventSelector eventSelector)
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
                KeySelectorType.Property => WithEventType(eventSelector, _ => _.EventPropertyKeySelector = new EventPropertyKeySelector { PropertyName = eventSelector.KeySelector.Expression ?? string.Empty }),
                _ => throw new UnknownKeySelectorType(eventSelector.KeySelector.Type)
            };
        }
    }
}

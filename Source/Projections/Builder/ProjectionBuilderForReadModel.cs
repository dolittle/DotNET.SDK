// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using Dolittle.SDK.Artifacts;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Processing;
using Dolittle.SDK.Projections.Internal;
using Dolittle.SDK.Projections.Store.Converters;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Projections.Builder
{
    /// <summary>
    /// Represents a builder for building projection on-methods.
    /// </summary>
    /// <typeparam name="TReadModel">The <see cref="Type" /> of the read model.</typeparam>
    public class ProjectionBuilderForReadModel<TReadModel> : ICanBuildAndRegisterAProjection
        where TReadModel : class, new()
    {
        readonly IList<IProjectionMethod<TReadModel>> _methods = new List<IProjectionMethod<TReadModel>>();
        readonly ProjectionId _projectionId;
        ScopeId _scopeId;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionBuilderForReadModel{TReadModel}"/> class.
        /// </summary>
        /// <param name="projectionId">The <see cref="ProjectionId" />.</param>
        /// <param name="scopeId">The <see cref="ScopeId" />.</param>
        public ProjectionBuilderForReadModel(ProjectionId projectionId, ScopeId scopeId)
        {
            _projectionId = projectionId;
            _scopeId = scopeId;
        }

        /// <summary>
        /// Defines the projection to operate in a specific <see cref="_scopeId" />.
        /// </summary>
        /// <param name="scopeId">The <see cref="_scopeId" />.</param>
        /// <returns>The builder for continuation.</returns>
        public ProjectionBuilderForReadModel<TReadModel> InScope(ScopeId scopeId)
        {
            _scopeId = scopeId;
            return this;
        }

        /// <summary>
        /// Add a method for updating a projection on an event.
        /// </summary>
        /// <typeparam name="TEvent">The <see cref="Type" /> of the event.</typeparam>
        /// <param name="selectorCallback">The <see cref="KeySelectorSignature{T}"/> used to build the <see cref="KeySelector"/> for the event.</param>
        /// <param name="method">The <see cref="TaskProjectionSignature{TReadModel, TEvent}" />.</param>
        /// <returns>The <see cref="ProjectionBuilderForReadModel{TReadModel}" /> for continuation.</returns>
        public ProjectionBuilderForReadModel<TReadModel> On<TEvent>(KeySelectorSignature<TEvent> selectorCallback, TaskProjectionSignature<TReadModel, TEvent> method)
            where TEvent : class
        {
            _methods.Add(new TypedProjectionMethod<TReadModel, TEvent>(method, selectorCallback(new KeySelectorBuilder<TEvent>())));
            return this;
        }

        /// <summary>
        /// Add a method for updating a projection on an event.
        /// </summary>
        /// <typeparam name="TEvent">The <see cref="Type" /> of the event.</typeparam>
        /// <param name="selectorCallback">The <see cref="KeySelectorSignature{T}"/> used to build the <see cref="KeySelector"/> for the event.</param>
        /// <param name="method">The <see cref="SyncProjectionSignature{T}" />.</param>
        /// <returns>The <see cref="ProjectionBuilderForReadModel{TReadModel}" /> for continuation.</returns>
        public ProjectionBuilderForReadModel<TReadModel> On<TEvent>(KeySelectorSignature<TEvent> selectorCallback, SyncProjectionSignature<TReadModel, TEvent> method)
            where TEvent : class
        {
            _methods.Add(new TypedProjectionMethod<TReadModel, TEvent>(method, selectorCallback(new KeySelectorBuilder<TEvent>())));
            return this;
        }

        /// <summary>
        /// Add a method for updating a projection on an event.
        /// </summary>
        /// <param name="eventType">The <see cref="EventType" /> of the event to handle.</param>
        /// <param name="selectorCallback">The <see cref="KeySelectorSignature"/> used to build the <see cref="KeySelector"/> for the event.</param>
        /// <param name="method">The <see cref="TaskProjectionSignature{TReadModel}" />.</param>
        /// <returns>The <see cref="ProjectionBuilderForReadModel{TReadModel}" /> for continuation.</returns>
        public ProjectionBuilderForReadModel<TReadModel> On(EventType eventType, KeySelectorSignature selectorCallback, TaskProjectionSignature<TReadModel> method)
        {
            _methods.Add(new ProjectionMethod<TReadModel>(method, eventType, selectorCallback(new KeySelectorBuilder())));
            return this;
        }

        /// <summary>
        /// Add a method for updating a projection on an event.
        /// </summary>
        /// <param name="eventType">The <see cref="EventType" /> of the event to handle.</param>
        /// <param name="selectorCallback">The <see cref="KeySelectorSignature{T}"/> used to build the <see cref="KeySelector"/> for the event.</param>
        /// <param name="method">The <see cref="SyncProjectionSignature{TReadModel}" />.</param>
        /// <returns>The <see cref="ProjectionBuilderForReadModel{TReadModel}" /> for continuation.</returns>
        public ProjectionBuilderForReadModel<TReadModel> On(EventType eventType, KeySelectorSignature selectorCallback, SyncProjectionSignature<TReadModel> method)
        {
            _methods.Add(new ProjectionMethod<TReadModel>(method, eventType, selectorCallback(new KeySelectorBuilder())));
            return this;
        }

        /// <summary>
        /// Add a method for updating a projection on an event.
        /// </summary>
        /// <param name="eventTypeId">The <see cref="EventTypeId" /> of the event to handle.</param>
        /// <param name="selectorCallback">The <see cref="KeySelectorSignature{T}"/> used to build the <see cref="KeySelector"/> for the event.</param>
        /// <param name="method">The <see cref="TaskProjectionSignature{TReadModel}" />.</param>
        /// <returns>The <see cref="ProjectionBuilderForReadModel{TReadModel}" /> for continuation.</returns>
        public ProjectionBuilderForReadModel<TReadModel> On(EventTypeId eventTypeId, KeySelectorSignature selectorCallback, TaskProjectionSignature<TReadModel> method)
            => On(new EventType(eventTypeId), selectorCallback, method);

        /// <summary>
        /// Add a method for updating a projection on an event.
        /// </summary>
        /// <param name="eventTypeId">The <see cref="EventTypeId" /> of the event to handle.</param>
        /// <param name="selectorCallback">The <see cref="KeySelectorSignature{T}"/> used to build the <see cref="KeySelector"/> for the event.</param>
        /// <param name="method">The <see cref="SyncProjectionSignature{TReadModel}" />.</param>
        /// <returns>The <see cref="ProjectionBuilderForReadModel{TReadModel}" /> for continuation.</returns>
        public ProjectionBuilderForReadModel<TReadModel> On(EventTypeId eventTypeId, KeySelectorSignature selectorCallback, SyncProjectionSignature<TReadModel> method)
            => On(new EventType(eventTypeId), selectorCallback, method);

        /// <summary>
        /// Add a method for updating a projection on an event.
        /// </summary>
        /// <param name="eventTypeId">The <see cref="EventTypeId" /> of the event to handle.</param>
        /// <param name="eventTypeGeneration">The <see cref="Generation" /> of the <see cref="EventType" /> of the event to handle.</param>
        /// <param name="selectorCallback">The <see cref="KeySelectorSignature{T}"/> used to build the <see cref="KeySelector"/> for the event.</param>
        /// <param name="method">The <see cref="TaskProjectionSignature{TReadModel}" />.</param>
        /// <returns>The <see cref="ProjectionBuilderForReadModel{TReadModel}" /> for continuation.</returns>
        public ProjectionBuilderForReadModel<TReadModel> On(EventTypeId eventTypeId, Generation eventTypeGeneration, KeySelectorSignature selectorCallback, TaskProjectionSignature<TReadModel> method)
            => On(new EventType(eventTypeId, eventTypeGeneration), selectorCallback, method);

        /// <summary>
        /// Add a method for updating a projection on an event.
        /// </summary>
        /// <param name="eventTypeId">The <see cref="EventTypeId" /> of the event to handle.</param>
        /// <param name="eventTypeGeneration">The <see cref="Generation" /> of the <see cref="EventType" /> of the event to handle.</param>
        /// <param name="selectorCallback">The <see cref="KeySelectorSignature{T}"/> used to build the <see cref="KeySelector"/> for the event.</param>
        /// <param name="method">The <see cref="SyncProjectionSignature{TReadModel}" />.</param>
        /// <returns>The <see cref="ProjectionBuilderForReadModel{TReadModel}" /> for continuation.</returns>
        public ProjectionBuilderForReadModel<TReadModel> On(EventTypeId eventTypeId, Generation eventTypeGeneration, KeySelectorSignature selectorCallback, SyncProjectionSignature<TReadModel> method)
            => On(new EventType(eventTypeId, eventTypeGeneration), selectorCallback, method);

        /// <summary>
        /// Builds the projection methods.
        /// </summary>
        /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
        /// <param name="eventTypesToMethods">The output <see cref="IDictionary{TKey, TValue}" /> of <see cref="EventType" /> to <see cref="IProjectionMethod{TReadModel}" /> map.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        /// <returns>Whether all the projection methods could be built.</returns>
        public bool TryAddOnMethods(IEventTypes eventTypes, IDictionary<EventType, IProjectionMethod<TReadModel>> eventTypesToMethods, ILogger logger)
        {
            var okay = true;
            foreach (var method in _methods)
            {
                var eventType = method.GetEventType(eventTypes);
                if (!eventTypesToMethods.TryAdd(eventType, method))
                {
                    okay = false;
                    logger.LogWarning(
                        "Projection {Projection} already handles event with event type {EventType}",
                        _projectionId,
                        eventType);
                }
            }

            return okay;
        }

        /// <inheritdoc/>
        public void BuildAndRegister(
            IEventProcessors eventProcessors,
            IEventTypes eventTypes,
            IEventProcessingConverter processingConverter,
            IConvertProjectionsToSDK projectionConverter,
            ILoggerFactory loggerFactory,
            CancellationToken cancelConnectToken,
            CancellationToken stopProcessorToken)
        {
            var logger = loggerFactory.CreateLogger<ProjectionBuilderForReadModel<TReadModel>>();
            var eventTypesToMethods = new Dictionary<EventType, IProjectionMethod<TReadModel>>();
            if (!TryAddOnMethods(eventTypes, eventTypesToMethods, logger))
            {
                logger.LogWarning(
                        "Failed to build projection {Projection}. One or more projection methods could not be built",
                        _projectionId);
                return;
            }

            if (eventTypesToMethods.Count < 1)
            {
                logger.LogWarning(
                        "Failed to build projection {Projection}. No projection methods are configured for projection",
                        _projectionId);
                return;
            }

            var projection = new Projection<TReadModel>(_projectionId, _scopeId, eventTypesToMethods);
            var projectionProcessor = new ProjectionsProcessor<TReadModel>(
                projection,
                processingConverter,
                projectionConverter,
                loggerFactory.CreateLogger<ProjectionsProcessor<TReadModel>>());

            eventProcessors.Register(
                projectionProcessor,
                new ProjectionsProtocol(),
                cancelConnectToken,
                stopProcessorToken);
        }
    }
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using Dolittle.SDK.Artifacts;
using Dolittle.SDK.Embeddings.Internal;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Processing;
using Dolittle.SDK.Events.Store.Converters;
using Dolittle.SDK.Projections.Store.Converters;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Embeddings.Builder
{
    /// <summary>
    /// Represents a builder for building an embeddings methods.
    /// </summary>
    /// <typeparam name="TReadModel">The <see cref="Type" /> of the read model.</typeparam>
    public class EmbeddingBuilderForReadModel<TReadModel> : ICanBuildAndRegisterAnEmbedding
        where TReadModel : class, new()
    {
        readonly IList<IOnMethod<TReadModel>> _methods = new List<IOnMethod<TReadModel>>();
        readonly EmbeddingId _embeddingId;
        ICompareMethod<TReadModel> _compareMethod;
        IRemoveMethod<TReadModel> _removeMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddingBuilderForReadModel{TReadModel}"/> class.
        /// </summary>
        /// <param name="embeddingId">The <see cref="EmbeddingId" />.</param>
        public EmbeddingBuilderForReadModel(EmbeddingId embeddingId)
        {
            _embeddingId = embeddingId;
        }

        /// <summary>
        /// Add the compare method for comparing the received and current state of the embedding.
        /// The method should return a single event, that changes the current state more towards the received state.
        /// This method will be called until the received state and current state are equal.
        /// </summary>
        /// <param name="method">The <see cref="CompareSignature{TReadModel}"/>.</param>
        /// <returns>The <see cref="EmbeddingBuilderForReadModel{TReadModel}" /> for continuation.</returns>
        public EmbeddingBuilderForReadModel<TReadModel> Compare(CompareSignature<TReadModel> method)
        {
            ThrowIfCompareMethodSet();
            _compareMethod = new CompareMethod<TReadModel>(method);
            return this;
        }

        /// <summary>
        /// Add the compare method for comparing the received and current state of the embedding.
        /// The method should return an enumerable of  events, that change the current state more towards the received state.
        /// This method will be called until the received state and current state are equal.
        /// </summary>
        /// <param name="method">The <see cref="CompareEnumerableReturnSignature{TReadModel}"/>.</param>
        /// <returns>The <see cref="EmbeddingBuilderForReadModel{TReadModel}" /> for continuation.</returns>
        public EmbeddingBuilderForReadModel<TReadModel> Compare(CompareEnumerableReturnSignature<TReadModel> method)
        {
            ThrowIfCompareMethodSet();
            _compareMethod = new CompareMethod<TReadModel>(method);
            return this;
        }

        /// <summary>
        /// Add the remove method for comparing the received and current state of the embedding.
        /// </summary>
        /// <param name="method">The <see cref="RemoveSignature{TReadModel}"/>.</param>
        /// <returns>The <see cref="EmbeddingBuilderForReadModel{TReadModel}" /> for continuation.</returns>
        public EmbeddingBuilderForReadModel<TReadModel> Remove(RemoveSignature<TReadModel> method)
        {
            ThrowIfRemoveMethodSet();
            _removeMethod = new RemoveMethod<TReadModel>(method);
            return this;
        }

        /// <summary>
        /// Add the remove method for comparing the received and current state of the embedding.
        /// </summary>
        /// <param name="method">The <see cref="RemoveSignature{TReadModel}"/>.</param>
        /// <returns>The <see cref="EmbeddingBuilderForReadModel{TReadModel}" /> for continuation.</returns>
        public EmbeddingBuilderForReadModel<TReadModel> Remove(RemoveEnumerableReturnSignature<TReadModel> method)
        {
            ThrowIfRemoveMethodSet();
            _removeMethod = new RemoveMethod<TReadModel>(method);
            return this;
        }

        /// <summary>
        /// Add a method for updating a projection on an event.
        /// </summary>
        /// <typeparam name="TEvent">The <see cref="Type" /> of the event.</typeparam>
        /// <param name="method">The <see cref="TaskOnSignature{TReadModel, TEvent}" />.</param>
        /// <returns>The <see cref="EmbeddingBuilderForReadModel{TReadModel}" /> for continuation.</returns>
        public EmbeddingBuilderForReadModel<TReadModel> On<TEvent>(TaskOnSignature<TReadModel, TEvent> method)
            where TEvent : class
        {
            _methods.Add(new TypedOnMethod<TReadModel, TEvent>(method));
            return this;
        }

        /// <summary>
        /// Add a method for updating a projection on an event.
        /// </summary>
        /// <typeparam name="TEvent">The <see cref="Type" /> of the event.</typeparam>
        /// <param name="method">The <see cref="SyncOnSignature{T}" />.</param>
        /// <returns>The <see cref="EmbeddingBuilderForReadModel{TReadModel}" /> for continuation.</returns>
        public EmbeddingBuilderForReadModel<TReadModel> On<TEvent>(SyncOnSignature<TReadModel, TEvent> method)
            where TEvent : class
        {
            _methods.Add(new TypedOnMethod<TReadModel, TEvent>(method));
            return this;
        }

        /// <summary>
        /// Add a method for updating a projection on an event.
        /// </summary>
        /// <param name="eventType">The <see cref="EventType" /> of the event to handle.</param>
        /// <param name="method">The <see cref="TaskOnSignature{TReadModel}" />.</param>
        /// <returns>The <see cref="EmbeddingBuilderForReadModel{TReadModel}" /> for continuation.</returns>
        public EmbeddingBuilderForReadModel<TReadModel> On(EventType eventType, TaskOnSignature<TReadModel> method)
        {
            _methods.Add(new OnMethod<TReadModel>(method, eventType));
            return this;
        }

        /// <summary>
        /// Add a method for updating a projection on an event.
        /// </summary>
        /// <param name="eventType">The <see cref="EventType" /> of the event to handle.</param>
        /// <param name="method">The <see cref="SyncOnSignature{TReadModel}" />.</param>
        /// <returns>The <see cref="EmbeddingBuilderForReadModel{TReadModel}" /> for continuation.</returns>
        public EmbeddingBuilderForReadModel<TReadModel> On(EventType eventType, SyncOnSignature<TReadModel> method)
        {
            _methods.Add(new OnMethod<TReadModel>(method, eventType));
            return this;
        }

        /// <summary>
        /// Add a method for updating a projection on an event.
        /// </summary>
        /// <param name="eventTypeId">The <see cref="EventTypeId" /> of the event to handle.</param>
        /// <param name="method">The <see cref="TaskOnSignature{TReadModel}" />.</param>
        /// <returns>The <see cref="EmbeddingBuilderForReadModel{TReadModel}" /> for continuation.</returns>
        public EmbeddingBuilderForReadModel<TReadModel> On(EventTypeId eventTypeId, TaskOnSignature<TReadModel> method)
            => On(new EventType(eventTypeId), method);

        /// <summary>
        /// Add a method for updating a projection on an event.
        /// </summary>
        /// <param name="eventTypeId">The <see cref="EventTypeId" /> of the event to handle.</param>
        /// <param name="method">The <see cref="SyncOnSignature{TReadModel}" />.</param>
        /// <returns>The <see cref="EmbeddingBuilderForReadModel{TReadModel}" /> for continuation.</returns>
        public EmbeddingBuilderForReadModel<TReadModel> On(EventTypeId eventTypeId, SyncOnSignature<TReadModel> method)
            => On(new EventType(eventTypeId), method);

        /// <summary>
        /// Add a method for updating a projection on an event.
        /// </summary>
        /// <param name="eventTypeId">The <see cref="EventTypeId" /> of the event to handle.</param>
        /// <param name="eventTypeGeneration">The <see cref="Generation" /> of the <see cref="EventType" /> of the event to handle.</param>
        /// <param name="method">The <see cref="TaskOnSignature{TReadModel}" />.</param>
        /// <returns>The <see cref="EmbeddingBuilderForReadModel{TReadModel}" /> for continuation.</returns>
        public EmbeddingBuilderForReadModel<TReadModel> On(EventTypeId eventTypeId, Generation eventTypeGeneration, TaskOnSignature<TReadModel> method)
            => On(new EventType(eventTypeId, eventTypeGeneration), method);

        /// <summary>
        /// Add a method for updating a projection on an event.
        /// </summary>
        /// <param name="eventTypeId">The <see cref="EventTypeId" /> of the event to handle.</param>
        /// <param name="eventTypeGeneration">The <see cref="Generation" /> of the <see cref="EventType" /> of the event to handle.</param>
        /// <param name="method">The <see cref="SyncOnSignature{TReadModel}" />.</param>
        /// <returns>The <see cref="EmbeddingBuilderForReadModel{TReadModel}" /> for continuation.</returns>
        public EmbeddingBuilderForReadModel<TReadModel> On(EventTypeId eventTypeId, Generation eventTypeGeneration, SyncOnSignature<TReadModel> method)
            => On(new EventType(eventTypeId, eventTypeGeneration), method);

        /// <inheritdoc/>
        public void BuildAndRegister(
            IEventProcessors eventProcessors,
            IEventTypes eventTypes,
            IConvertEventsToProtobuf eventsToProtobufConverter,
            IConvertProjectionsToSDK projectionsConverter,
            ILoggerFactory loggerFactory,
            CancellationToken cancellation)
        {
            var logger = loggerFactory.CreateLogger<EmbeddingBuilderForReadModel<TReadModel>>();
            var eventTypesToMethods = new Dictionary<EventType, IOnMethod<TReadModel>>();
            if (!TryAddOnMethods(eventTypes, eventTypesToMethods, logger))
            {
                logger.LogWarning(
                        "Failed to build embedding {Embedding}. One or more on methods could not be built",
                        _embeddingId);
                return;
            }

            if (eventTypesToMethods.Count < 1)
            {
                logger.LogWarning(
                        "Failed to build embedding {Embedding}. No on methods are configured for embedding",
                        _embeddingId);
                return;
            }

            var embedding = new Embedding<TReadModel>(
                _embeddingId,
                eventTypes,
                eventTypesToMethods,
                _compareMethod,
                _removeMethod);
            var embeddingsProcessor = new EmbeddingsProcessor<TReadModel>(
                embedding,
                eventsToProtobufConverter,
                projectionsConverter,
                eventTypes,
                loggerFactory.CreateLogger<EmbeddingsProcessor<TReadModel>>());

            eventProcessors.Register(
                embeddingsProcessor,
                new EmbeddingsProtocol(),
                cancellation);
        }

        bool TryAddOnMethods(IEventTypes eventTypes, IDictionary<EventType, IOnMethod<TReadModel>> eventTypesToMethods, ILogger logger)
        {
            var okay = true;
            foreach (var method in _methods)
            {
                var eventType = method.GetEventType(eventTypes);
                if (!eventTypesToMethods.TryAdd(eventType, method))
                {
                    okay = false;
                    logger.LogWarning(
                        "Embedding {Embedding} already handles event with event type {EventType}",
                        _embeddingId,
                        eventType);
                }
            }

            return okay;
        }

        void ThrowIfCompareMethodSet()
        {
            if (_compareMethod != default)
            {
                throw new EmbeddingAlreadyHasACompareMethod(_embeddingId);
            }
        }

        void ThrowIfRemoveMethodSet()
        {
            if (_removeMethod != default)
            {
                throw new EmbeddingAlreadyHasARemoveMethod(_embeddingId);
            }
        }
    }
}

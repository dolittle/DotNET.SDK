// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using Dolittle.SDK.Embeddings.Internal;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Processing;
using Dolittle.SDK.Events.Store.Converters;
using Dolittle.SDK.Projections.Store.Converters;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Embeddings.Builder
{
    /// <summary>
    /// Methods for building <see cref="IEmbedding{TReadModel}"/> instances by convention from an instantiated embedding class.
    /// </summary>
    /// <typeparam name="TEmbedding">The <see cref="Type" /> of the embedding.</typeparam>
    public class ConventionEmbeddingBuilder<TEmbedding> : ICanBuildAndRegisterAnEmbedding
        where TEmbedding : class, new()
    {
        readonly Type _embeddingType = typeof(TEmbedding);

        /// <inheritdoc/>
        public void BuildAndRegister(
            IEventProcessors eventProcessors,
            IEventTypes eventTypes,
            IConvertEventsToProtobuf eventsToProtobufConverter,
            IConvertProjectionsToSDK projectionsConverter,
            ILoggerFactory loggerFactory,
            CancellationToken cancellation)
        {
            var logger = loggerFactory.CreateLogger(GetType());
            logger.LogDebug("Building embedding from type {EmbeddingType}", _embeddingType);

            if (!HasParameterlessConstructor())
            {
                logger.LogWarning("The embedding class {EmbeddingType} has no default/parameterless constructor", _embeddingType);
                return;
            }

            if (HasMoreThanOneConstructor())
            {
                logger.LogWarning("The embedding class {EmbeddingType} has more than one constructor. It must only have one, parameterless, constructor", _embeddingType);
                return;
            }

            if (!TryGetEmbeddingId(out var embeddingId))
            {
                logger.LogWarning("The embedding class {EmbeddingType} needs to be decorated with an [{EmbeddingAttribute}]", _embeddingType, typeof(EmbeddingAttribute).Name);
                return;
            }

            logger.LogTrace(
                "Building embedding {Embedding} from type {EmbeddingType}",
                embeddingId,
                _embeddingType);

            if (!ClassMethodBuilder<TEmbedding>
                .ForProjection(embeddingId, eventTypes, loggerFactory)
                .TryBuild(out var eventTypesToMethods))
            {
                return;
            }

            if (!ClassMethodBuilder<TEmbedding>
                .ForUpdate(embeddingId, eventTypes, loggerFactory)
                .TryBuild(out var updateMethod))
            {
                return;
            }

            if (!ClassMethodBuilder<TEmbedding>
                .ForDelete(embeddingId, eventTypes, loggerFactory)
                .TryBuild(out var deleteMethod))
            {
                return;
            }

            var embedding = new Embedding<TEmbedding>(
                embeddingId,
                eventTypes,
                eventTypesToMethods,
                updateMethod,
                deleteMethod);
            var embeddingsProcessor = new EmbeddingsProcessor<TEmbedding>(
                embedding,
                eventsToProtobufConverter,
                projectionsConverter,
                eventTypes,
                loggerFactory.CreateLogger<EmbeddingsProcessor<TEmbedding>>());

            eventProcessors.Register(
                embeddingsProcessor,
                new EmbeddingsProtocol(),
                cancellation);
        }

        bool TryGetEmbeddingId(out EmbeddingId embeddingId)
        {
            embeddingId = default;
            var embedding = _embeddingType.GetCustomAttributes(typeof(EmbeddingAttribute), true).FirstOrDefault() as EmbeddingAttribute;
            if (embedding == default) return false;

            embeddingId = embedding.Identifier;
            return true;
        }

        bool HasMoreThanOneConstructor()
            => _embeddingType.GetConstructors().Length > 1;

        bool HasParameterlessConstructor()
            => _embeddingType.GetConstructors().Any(t => t.GetParameters().Length == 0);
    }
}

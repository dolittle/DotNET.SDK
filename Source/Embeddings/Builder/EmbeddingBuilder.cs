// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.SDK.Embeddings.Store;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Processing;
using Dolittle.SDK.Events.Store.Converters;
using Dolittle.SDK.Projections.Store.Converters;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Embeddings.Builder
{
    /// <summary>
    /// Represents a builder for building an embedding.
    /// </summary>
    public class EmbeddingBuilder : ICanBuildAndRegisterAnEmbedding
    {
        readonly EmbeddingId _embeddingId;
        readonly IEmbeddingReadModelTypeAssociations _embeddingAssociations;
        ICanBuildAndRegisterAnEmbedding _methodsBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddingBuilder"/> class.
        /// </summary>
        /// <param name="embeddingId">The <see cref="EmbeddingId" />.</param>
        /// <param name="embeddingAssociations">The <see cref="IEmbeddingReadModelTypeAssociations" />.</param>
        public EmbeddingBuilder(EmbeddingId embeddingId, IEmbeddingReadModelTypeAssociations embeddingAssociations)
        {
            _embeddingId = embeddingId;
            _embeddingAssociations = embeddingAssociations;
        }

        /// <summary>
        /// Creates a <see cref="EmbeddingBuilderForReadModel{TReadModel}" /> for the specified read model type.
        /// </summary>
        /// <typeparam name="TReadModel">The <see cref="Type" /> of the read model.</typeparam>
        /// <returns>The <see cref="EmbeddingBuilderForReadModel{TReadModel}" /> for continuation.</returns>
        /// <exception cref="ReadModelAlreadyDefinedForEmbedding">Exception.</exception>
        public EmbeddingBuilderForReadModel<TReadModel> ForReadModel<TReadModel>()
            where TReadModel : class, new()
        {
            if (_methodsBuilder != default)
            {
                throw new ReadModelAlreadyDefinedForEmbedding(_embeddingId, typeof(TReadModel));
            }

            _embeddingAssociations.Associate<TReadModel>(_embeddingId.Value);
            var builder = new EmbeddingBuilderForReadModel<TReadModel>(_embeddingId);
            _methodsBuilder = builder;
            return builder;
        }

        /// <inheritdoc/>
        public void BuildAndRegister(
            IEventProcessors eventProcessors,
            IEventTypes eventTypes,
            IConvertEventsToProtobuf eventsToProtobufConverter,
            IConvertProjectionsToSDK projectionsConverter,
            ILoggerFactory loggerFactory,
            CancellationToken cancellation)
        {
            if (_methodsBuilder == null)
            {
                throw new EmbeddingNeedsToBeForReadModel(_embeddingId);
            }

            _methodsBuilder.BuildAndRegister(eventProcessors, eventTypes, eventsToProtobufConverter, projectionsConverter, loggerFactory, cancellation);
        }
    }
}

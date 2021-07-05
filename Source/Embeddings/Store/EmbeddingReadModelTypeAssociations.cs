// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Dolittle.SDK.Embeddings.Store
{
    /// <summary>
    /// Represents an implementation of <see cref="IEmbeddingReadModelTypeAssociations" />.
    /// </summary>
    public class EmbeddingReadModelTypeAssociations : IEmbeddingReadModelTypeAssociations
    {
        readonly Dictionary<Type, EmbeddingId> _typeToAssociations = new Dictionary<Type, EmbeddingId>();

        /// <inheritdoc/>
        public void Associate(EmbeddingId embedding, Type embeddingType)
        {
            ThrowIfMultipleProjectionsAssociatedWithType(embeddingType, embedding);
            _typeToAssociations.Add(embeddingType, embedding);
        }

        /// <inheritdoc/>
        public void Associate<TReadModel>(EmbeddingId embedding)
            where TReadModel : class, new()
            => Associate(embedding, typeof(TReadModel));

        /// <inheritdoc/>
        public void Associate<TEmbedding>()
            where TEmbedding : class, new()
            => Associate(typeof(TEmbedding));

        /// <inheritdoc/>
        public void Associate(Type embeddingType)
        {
            var embeddingAttribute = embeddingType
                .GetCustomAttributes(typeof(EmbeddingAttribute), true)
                .FirstOrDefault() as EmbeddingAttribute;

            if (embeddingAttribute == default)
            {
                throw new TypeIsNotAnEmbedding(embeddingType);
            }

            Associate(embeddingAttribute.Identifier, embeddingType);
        }

        /// <inheritdoc/>
        public EmbeddingId GetFor<TEmbedding>()
            where TEmbedding : class, new()
        {
            if (!_typeToAssociations.TryGetValue(typeof(TEmbedding), out var association))
            {
                throw new NoEmbeddingAssociatedWithType(typeof(TEmbedding));
            }

            return association;
        }

        void ThrowIfMultipleProjectionsAssociatedWithType(Type projectionType, EmbeddingId projection)
        {
            if (_typeToAssociations.TryGetValue(projectionType, out var existingId))
            {
                throw new CannotAssociateMultipleEmbeddingsWithType(projectionType, projection, existingId);
            }
        }
    }
}

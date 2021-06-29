// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Embeddings.Internal
{
    /// <summary>
    /// Represents the result of a embeddings' On-method.
    /// </summary>
    /// <typeparam name="TReadModel">The type of the read model.</typeparam>
    public class EmbeddingResult<TReadModel>
        where TReadModel : class, new()
    {
        /// <summary>
        /// Gets a <see cref="ProjectionResult{TReadModel}" /> that signifies that the read model should be deleted.
        /// </summary>
        public static EmbeddingResult<TReadModel> Delete
            => new EmbeddingResult<TReadModel>
            {
                Type = EmbeddingResultType.Delete
            };

        /// <summary>
        /// Gets the updated read model.
        /// </summary>
        public TReadModel UpdatedReadModel { get; private set; }

        /// <summary>
        /// Gets the <see cref="EmbeddingResultType" />.
        /// </summary>
        public EmbeddingResultType Type { get; private set; }

        /// <summary>
        /// Implicitly converts from a <typeparamref name="TReadModel"/> to a <see cref="EmbeddingResult{TReadModel}" />.
        /// </summary>
        /// <param name="readModel">The updated read model instance.</param>
        public static implicit operator EmbeddingResult<TReadModel>(TReadModel readModel)
            => new EmbeddingResult<TReadModel> { UpdatedReadModel = readModel, Type = EmbeddingResultType.Replace };
    }
}

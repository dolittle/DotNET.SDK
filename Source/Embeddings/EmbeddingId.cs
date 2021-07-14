// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Embeddings
{
    /// <summary>
    /// Represents the concept of a unique identifier for an embedding.
    /// </summary>
    public class EmbeddingId : ConceptAs<Guid>
    {
        /// <summary>
        /// Implicitly converts from a <see cref="Guid"/> to an <see cref="EmbeddingId"/>.
        /// </summary>
        /// <param name="projection">The <see cref="Guid"/> representation.</param>
        /// <returns>The converted <see cref="EmbeddingId"/>.</returns>
        public static implicit operator EmbeddingId(Guid projection) => new EmbeddingId { Value = projection };

        /// <summary>
        /// Implicitly converts from a <see cref="string"/> to an <see cref="EmbeddingId"/>.
        /// </summary>
        /// <param name="projection">The <see cref="string"/> representation.</param>
        /// <returns>The converted <see cref="EmbeddingId"/>.</returns>
        public static implicit operator EmbeddingId(string projection) => new EmbeddingId { Value = Guid.Parse(projection) };
    }
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Embeddings.Store
{
    /// <summary>
    /// Exception that gets thrown when trying to associate a type that isn't an embedding.
    /// </summary>
    public class TypeIsNotAnEmbedding : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeIsNotAnEmbedding"/> class.
        /// </summary>
        /// <param name="type">The type trying to associate.</param>
        public TypeIsNotAnEmbedding(Type type)
            : base($"Type {type} is not an embedding. Did you add the [Embedding()] attribute to it?")
        {
        }
    }
}

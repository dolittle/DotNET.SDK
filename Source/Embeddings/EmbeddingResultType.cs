// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Embeddings
{
    /// <summary>
    /// Defines the different result types of an embedding's on-methods.
    /// </summary>
    public enum EmbeddingResultType
    {
        /// <summary>
        /// Replaces the read model.
        /// </summary>
        Replace = 0,

        /// <summary>
        /// Deletes the read model.
        /// </summary>
        Delete = 1,
    }
}

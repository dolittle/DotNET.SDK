// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Concepts;
using Dolittle.SDK.Projections;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.Embeddings
{
    /// <summary>
    /// Represents the context of an embedding.
    /// </summary>
    public class EmbeddingContext : Value<EmbeddingContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddingContext"/> class.
        /// </summary>
        /// <param name="wasCreatedFromInitialState">A value indicating whether the embedding was created from the initial state or retrieved from a persisted state.</param>
        /// <param name="key">The embedding projection <see cref="Key"/>.</param>
        /// <param name="isDelete">Whether this is a call for a deletion.</param>
        /// <param name="executionContext">The <see cref="Execution.ExecutionContext"/> in which the event was committed in.</param>
        public EmbeddingContext(
            bool wasCreatedFromInitialState,
            Key key,
            bool isDelete,
            ExecutionContext executionContext)
        {
            WasCreatedFromInitialState = wasCreatedFromInitialState;
            Key = key;
            IsDelete = isDelete;
            ExecutionContext = executionContext;
        }

        /// <summary>
        /// Gets a value indicating whether the projection was created from the initial state or retrieved from a persisted state.
        /// </summary>
        public bool WasCreatedFromInitialState { get; }

        /// <summary>
        /// Gets the projection <see cref="Key"/>.
        /// </summary>
        public Key Key { get; }

        /// <summary>
        /// Gets a value indicating whether this is a call for deletion.
        /// </summary>
        public bool IsDelete { get; }

        /// <summary>
        /// Gets the <see cref="Execution.ExecutionContext"/> in which the event was committed in.
        /// </summary>
        public ExecutionContext ExecutionContext { get; }
    }
}
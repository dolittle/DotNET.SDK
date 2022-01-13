// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Projections;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.Embeddings;

/// <summary>
/// Represents the context of an embedding.
/// </summary>
/// <param name="WasCreatedFromInitialState">A value indicating whether the embedding was created from the initial state or retrieved from a persisted state.</param>
/// <param name="Key">The projection <see cref="Key"/>.</param>
/// <param name="IsDelete">Whether this is a call for a deletion.</param>
/// <param name="ExecutionContext">The <see cref="Execution.ExecutionContext"/> in which the event was committed in.</param>
public record EmbeddingContext(bool WasCreatedFromInitialState, Key Key, bool IsDelete, ExecutionContext ExecutionContext);

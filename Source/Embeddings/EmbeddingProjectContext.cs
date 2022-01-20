// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;
using Dolittle.SDK.Projections;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.Embeddings;

/// <summary>
/// Represents the context of an embeddings projection.
/// </summary>
/// <param name="WasCreatedFromInitialState">A value indicating whether the projection was created from the initial state or retrieved from a persisted state.</param>
/// <param name="Key">Gets the embedding <see cref="Key"/>.</param>
/// <param name="EventSourceId">Gets the <see cref="EventSourceId"/> in which the event comes from.</param>
/// <param name="ExecutionContext">Gets the <see cref="Execution.ExecutionContext"/> for this embedding.</param>
public record EmbeddingProjectContext(bool WasCreatedFromInitialState, Key Key, EventSourceId EventSourceId, ExecutionContext ExecutionContext);

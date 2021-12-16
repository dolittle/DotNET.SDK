// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections.Store;

/// <summary>
/// Represents an identifier for a scoped projection.
/// </summary>
/// <param name="Identifier">The unique identifier for projection - <see cref="ProjectionId" />.</param>
/// <param name="ScopeId">The scope the projection is in.</param>
public record ScopedProjectionIdentifier(ProjectionId Identifier, ScopeId ScopeId);

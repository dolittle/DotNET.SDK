// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Projections.Contracts;
using Dolittle.SDK.Events;
using Dolittle.SDK.Execution;

namespace Dolittle.SDK.Projections.Store;

/// <summary>
/// Defines a factory for <see cref="ProjectionStore"/> <see cref="Dolittle.Runtime.Projections.Contracts.Projections.ProjectionsClient"/>.
/// </summary>
public interface ICreateProjectionStoreRequest
{
    /// <summary>
    /// Creates the <see cref="GetOneRequest"/>.
    /// </summary>
    /// <param name="key">The <see cref="Key"/> of the projection state.</param>
    /// <param name="id">The <see cref="ProjectionId"/>.</param>
    /// <param name="scope">The <see cref="ScopeId"/>.</param>
    /// <param name="executionContext">The <see cref="ExecutionContext"/>.</param>
    /// <returns>The created <see cref="GetOneRequest"/> request.</returns>
    public GetOneRequest CreateGetOne(Key key, ProjectionId id, ScopeId scope, ExecutionContext executionContext);

    /// <summary>
    /// /// Creates the <see cref="GetAllRequest"/>.
    /// </summary>
    /// <param name="id">The <see cref="ProjectionId"/>.</param>
    /// <param name="executionContext">The <see cref="ExecutionContext"/>.</param>
    /// <returns>The created <see cref="GetAllRequest"/> request.</returns>
    public GetAllRequest CreateGetAll(ProjectionId id, ScopeId scope, ExecutionContext executionContext);
}

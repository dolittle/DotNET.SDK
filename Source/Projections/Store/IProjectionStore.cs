// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections.Store;

/// <summary>
/// Defines an interface for working directly with Projections.
/// </summary>
public interface IProjectionStore
{
    /// <summary>
    /// Gets a projection read model by key for a projection associated with a type.
    /// </summary>
    /// <param name="key">THe <see cref="Key" /> of the projection.</param>
    /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    /// <typeparam name="TProjection">The <see cref="Type" /> of the projection.</typeparam>
    /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <typeparamref name="TProjection"/> read model.</returns>
    Task<TProjection> Get<TProjection>(Key key, CancellationToken cancellation = default)
        where TProjection : class, new();

    /// <summary>
    /// Gets a projection read model by key for a projection specified by projection identifier.
    /// </summary>
    /// <param name="key">THe <see cref="Key" /> of the projection.</param>
    /// <param name="projectionId">The <see cref="ProjectionId"/>.</param>
    /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    /// <typeparam name="TProjection">The <see cref="Type" /> of the projection.</typeparam>
    /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <typeparamref name="TProjection"/> read model.</returns>
    Task<TProjection> Get<TProjection>(Key key, ProjectionId projectionId, CancellationToken cancellation = default)
        where TProjection : class, new();

    /// <summary>
    /// Gets a projection read model by key for a projection specified by projection identifier.
    /// </summary>
    /// <param name="key">THe <see cref="Key" /> of the projection.</param>
    /// <param name="projectionId">The <see cref="ProjectionId"/>.</param>
    /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <see cref="object"/> read model.</returns>
    Task<object> Get(Key key, ProjectionId projectionId, CancellationToken cancellation = default);

    /// <summary>
    /// Gets a projection read model by key for a projection specified by projection and scope identifier.
    /// </summary>
    /// <param name="key">THe <see cref="Key" /> of the projection.</param>
    /// <param name="projectionId">The <see cref="ProjectionId"/>.</param>
    /// <param name="scopeId">The <see cref="ScopeId"/>.</param>
    /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    /// <typeparam name="TProjection">The <see cref="Type" /> of the projection.</typeparam>
    /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <typeparamref name="TProjection"/> read model.</returns>
    Task<TProjection> Get<TProjection>(Key key, ProjectionId projectionId, ScopeId scopeId, CancellationToken cancellation = default)
        where TProjection : class, new();

    /// <summary>
    /// Gets a projection read model by key for a projection specified by projection and scope identifier.
    /// </summary>
    /// <param name="key">THe <see cref="Key" /> of the projection.</param>
    /// <param name="projectionId">The <see cref="ProjectionId"/>.</param>
    /// <param name="scopeId">The <see cref="ScopeId"/>.</param>
    /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <see cref="object"/> read model.</returns>
    Task<object> Get(Key key, ProjectionId projectionId, ScopeId scopeId, CancellationToken cancellation = default);

    /// <summary>
    /// Gets all projection read models for a projection associated with a type.
    /// </summary>
    /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    /// <typeparam name="TProjection">The <see cref="Type" /> of the projection.</typeparam>
    /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <see cref="IEnumerable{T}" /> of <typeparamref name="TProjection" />.</returns>
    Task<IEnumerable<TProjection>> GetAll<TProjection>(CancellationToken cancellation = default)
        where TProjection : class, new();

    /// <summary>
    /// Gets all projection read models for a projection specified by projection identifier.
    /// </summary>
    /// <param name="projectionId">The <see cref="ProjectionId"/>.</param>
    /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    /// <typeparam name="TProjection">The <see cref="Type" /> of the projection.</typeparam>
    /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <see cref="IEnumerable{T}" /> of <typeparamref name="TProjection" />.</returns>
    Task<IEnumerable<TProjection>> GetAll<TProjection>(ProjectionId projectionId, CancellationToken cancellation = default)
        where TProjection : class, new();

    /// <summary>
    /// Gets all projection read models for a projection specified by projection identifier.
    /// </summary>
    /// <param name="projectionId">The <see cref="ProjectionId"/>.</param>
    /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <see cref="IEnumerable{T}" /> of <see cref="object" />.</returns>
    Task<IEnumerable<object>> GetAll(ProjectionId projectionId, CancellationToken cancellation = default);

    /// <summary>
    /// Gets all projection read models for a projection specified by projection and scope identifier.
    /// </summary>
    /// <param name="projectionId">The <see cref="ProjectionId"/>.</param>
    /// <param name="scopeId">The <see cref="ScopeId"/>.</param>
    /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    /// <typeparam name="TProjection">The <see cref="Type" /> of the projection.</typeparam>
    /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <see cref="IEnumerable{T}" /> of <typeparamref name="TProjection" />.</returns>
    Task<IEnumerable<TProjection>> GetAll<TProjection>(ProjectionId projectionId, ScopeId scopeId, CancellationToken cancellation = default)
        where TProjection : class, new();

    /// <summary>
    /// Gets all projection read models for a projection specified by projection and scope identifier.
    /// </summary>
    /// <param name="projectionId">The <see cref="ProjectionId"/>.</param>
    /// <param name="scopeId">The <see cref="ScopeId"/>.</param>
    /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <see cref="IEnumerable{T}" /> of <see cref="object" />.</returns>
    Task<IEnumerable<object>> GetAll(ProjectionId projectionId, ScopeId scopeId, CancellationToken cancellation = default);
}

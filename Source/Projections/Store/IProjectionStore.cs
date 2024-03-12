// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Gets the <see cref="IProjectionOf{TProjection}"/> for the <typeparamref name="TProjection"/> projection read model class.
    /// </summary>
    /// <typeparam name="TProjection">The <see cref="Type" /> of the projection.</typeparam>
    /// <returns>The <see cref="IProjectionOf{TProjection}"/> for the <typeparamref name="TProjection"/> projection read model class.</returns>
    IProjectionOf<TProjection> Of<TProjection>()
        where TProjection : ReadModel, new();

    /// <summary>
    /// Gets the <see cref="IProjectionOf{TReadModel}"/> for the <typeparamref name="TReadModel"/> projection read model class.
    /// </summary>
    /// <param name="projectionId">The <see cref="ProjectionId"/>.</param>
    /// <typeparam name="TReadModel">The <see cref="Type" /> of the projection read model.</typeparam>
    /// <returns>The <see cref="IProjectionOf{TReadModel}"/> for the <typeparamref name="TReadModel"/> projection read model class.</returns>
    IProjectionOf<TReadModel> Of<TReadModel>(ProjectionId projectionId)
        where TReadModel : ReadModel, new();

    /// <summary>
    /// Gets the <see cref="IProjectionOf{TReadModel}"/> for the <typeparamref name="TReadModel"/> projection read model class.
    /// </summary>
    /// <param name="projectionId">The <see cref="ProjectionId"/>.</param>
    /// <param name="scopeId">The <see cref="ScopeId"/>.</param>
    /// <typeparam name="TReadModel">The <see cref="Type" /> of the projection read model.</typeparam>
    /// <returns>The <see cref="IProjectionOf{TReadModel}"/> for the <typeparamref name="TReadModel"/> projection read model class.</returns>
    IProjectionOf<TReadModel> Of<TReadModel>(ProjectionId projectionId, ScopeId scopeId) where TReadModel : ReadModel, new();

    /// <summary>
    /// This will allow you to query the state of a projection at a specific point in time, or all states up to a specific point in time.
    /// Only available if the projection uses only events keyed on EventSourceId (default behavior).
    /// </summary>
    /// <returns>The <see cref="IHistoricalProjection{TProjection}"/> for the <typeparamref name="TProjection"/> projection read model.</returns>
    public IHistoricalProjection<TProjection> Historical<TProjection>() where TProjection : ReadModel, new();
    
    /// <summary>
    /// Gets a projection read model by key for a projection associated with a type.
    /// </summary>
    /// <param name="key">The <see cref="Key" /> of the projection.</param>
    /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    /// <typeparam name="TProjection">The <see cref="Type" /> of the projection.</typeparam>
    /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <typeparamref name="TProjection"/> read model.</returns>
    Task<TProjection?> Get<TProjection>(Key key, CancellationToken cancellation = default) where TProjection : ReadModel, new();
    
    /// <summary>
    /// Gets a projection read model by key for a projection specified by projection identifier.
    /// </summary>
    /// <param name="key">The <see cref="Key" /> of the projection.</param>
    /// <param name="projectionId">The <see cref="ProjectionId"/>.</param>
    /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    /// <typeparam name="TReadModel">The <see cref="Type" /> of the projection.</typeparam>
    /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <typeparamref name="TReadModel"/> read model.</returns>
    Task<TReadModel?> Get<TReadModel>(Key key, ProjectionId projectionId, CancellationToken cancellation = default)
        where TReadModel : ReadModel, new();
    
    /// <summary>
    /// Gets a projection read model by key for a projection specified by projection and scope identifier.
    /// </summary>
    /// <param name="key">The <see cref="Key" /> of the projection.</param>
    /// <param name="projectionId">The <see cref="ProjectionId"/>.</param>
    /// <param name="scopeId">The <see cref="ScopeId"/>.</param>
    /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    /// <typeparam name="TReadModel">The <see cref="Type" /> of the projection.</typeparam>
    /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <typeparamref name="TReadModel"/> read model.</returns>
    Task<TReadModel?> Get<TReadModel>(Key key, ProjectionId projectionId, ScopeId scopeId, CancellationToken cancellation = default)
        where TReadModel : ReadModel, new();

    /// <summary>
    /// Gets all projection read models for a projection associated with a type.
    /// </summary>
    /// <typeparam name="TProjection">The <see cref="Type" /> of the projection.</typeparam>
    /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <see cref="IEnumerable{T}" /> of <typeparamref name="TProjection" />.</returns>
    IQueryable<TProjection> AsQueryable<TProjection>() where TProjection : ReadModel, new();

}

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
        where TProjection : ProjectionBase, new();

    /// <summary>
    /// Gets the <see cref="IProjectionOf{TReadModel}"/> for the <typeparamref name="TReadModel"/> projection read model class.
    /// </summary>
    /// <param name="projectionId">The <see cref="ProjectionId"/>.</param>
    /// <typeparam name="TReadModel">The <see cref="Type" /> of the projection read model.</typeparam>
    /// <returns>The <see cref="IProjectionOf{TReadModel}"/> for the <typeparamref name="TReadModel"/> projection read model class.</returns>
    IProjectionOf<TReadModel> Of<TReadModel>(ProjectionId projectionId)
        where TReadModel : ProjectionBase, new();

    /// <summary>
    /// Gets the <see cref="IProjectionOf{TReadModel}"/> for the <typeparamref name="TReadModel"/> projection read model class.
    /// </summary>
    /// <param name="projectionId">The <see cref="ProjectionId"/>.</param>
    /// <param name="scopeId">The <see cref="ScopeId"/>.</param>
    /// <typeparam name="TReadModel">The <see cref="Type" /> of the projection read model.</typeparam>
    /// <returns>The <see cref="IProjectionOf{TReadModel}"/> for the <typeparamref name="TReadModel"/> projection read model class.</returns>
    IProjectionOf<TReadModel> Of<TReadModel>(ProjectionId projectionId, ScopeId scopeId) where TReadModel : ProjectionBase, new();
    
    /// <summary>
    /// Gets a projection read model by key for a projection associated with a type.
    /// </summary>
    /// <param name="key">The <see cref="Key" /> of the projection.</param>
    /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    /// <typeparam name="TProjection">The <see cref="Type" /> of the projection.</typeparam>
    /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <typeparamref name="TProjection"/> read model.</returns>
    Task<TProjection?> Get<TProjection>(Key key, CancellationToken cancellation = default) where TProjection : ProjectionBase, new();
    
    /// <summary>
    /// Gets a projection read model by key for a projection specified by projection identifier.
    /// </summary>
    /// <param name="key">The <see cref="Key" /> of the projection.</param>
    /// <param name="projectionId">The <see cref="ProjectionId"/>.</param>
    /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    /// <typeparam name="TReadModel">The <see cref="Type" /> of the projection.</typeparam>
    /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <typeparamref name="TReadModel"/> read model.</returns>
    Task<TReadModel?> Get<TReadModel>(Key key, ProjectionId projectionId, CancellationToken cancellation = default)
        where TReadModel : ProjectionBase, new();
    
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
        where TReadModel : ProjectionBase, new();
    
    // /// <summary>
    // /// Gets a projection read model by key for a projection specified by projection identifier.
    // /// </summary>
    // /// <param name="key">The <see cref="Key" /> of the projection.</param>
    // /// <param name="projectionId">The <see cref="ProjectionId"/>.</param>
    // /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    // /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <see cref="object"/> read model.</returns>
    // Task<object> Get(Key key, ProjectionId projectionId, CancellationToken cancellation = default);

    // /// <summary>
    // /// Gets a projection read model by key for a projection specified by projection and scope identifier.
    // /// </summary>
    // /// <param name="key">The <see cref="Key" /> of the projection.</param>
    // /// <param name="projectionId">The <see cref="ProjectionId"/>.</param>
    // /// <param name="scopeId">The <see cref="ScopeId"/>.</param>
    // /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    // /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <see cref="object"/> read model.</returns>
    // Task<object> Get(Key key, ProjectionId projectionId, ScopeId scopeId, CancellationToken cancellation = default);
    //
    // /// <summary>
    // /// Gets a projection state by key for a projection associated with a type.
    // /// </summary>
    // /// <param name="key">The <see cref="Key" /> of the projection.</param>
    // /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    // /// <typeparam name="TProjection">The <see cref="Type" /> of the projection.</typeparam>
    // /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <see cref="CurrentState{TProjection}"/> of <typeparamref name="TProjection"/>.</returns>
    // Task<CurrentState<TProjection>> GetState<TProjection>(Key key, CancellationToken cancellation = default)
    //     where TProjection : class, new();
    //
    // /// <summary>
    // /// Gets a projection state by key for a projection specified by projection identifier.
    // /// </summary>
    // /// <param name="key">The <see cref="Key" /> of the projection.</param>
    // /// <param name="projectionId">The <see cref="ProjectionId"/>.</param>
    // /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    // /// <typeparam name="TReadModel">The <see cref="Type" /> of the projection.</typeparam>
    // /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <see cref="CurrentState{TReadModel}"/> of <typeparamref name="TReadModel"/>.</returns>
    // Task<CurrentState<TReadModel>> GetState<TReadModel>(Key key, ProjectionId projectionId, CancellationToken cancellation = default)
    //     where TReadModel : class, new();
    //
    // /// <summary>
    // /// Gets a projection state by key for a projection specified by projection identifier.
    // /// </summary>
    // /// <param name="key">The <see cref="Key" /> of the projection.</param>
    // /// <param name="projectionId">The <see cref="ProjectionId"/>.</param>
    // /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    // /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <see cref="CurrentState{TReadModel}"/> of <see cref="object"/>.</returns>
    // Task<CurrentState<object>> GetState(Key key, ProjectionId projectionId, CancellationToken cancellation = default);
    //
    // /// <summary>
    // /// Gets a projection state by key for a projection specified by projection and scope identifier.
    // /// </summary>
    // /// <param name="key">The <see cref="Key" /> of the projection.</param>
    // /// <param name="projectionId">The <see cref="ProjectionId"/>.</param>
    // /// <param name="scopeId">The <see cref="ScopeId"/>.</param>
    // /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    // /// <typeparam name="TReadModel">The <see cref="Type" /> of the projection.</typeparam>
    // /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <see cref="CurrentState{TReadModel}"/> of <typeparamref name="TReadModel"/>.</returns>
    // Task<CurrentState<TReadModel>> GetState<TReadModel>(Key key, ProjectionId projectionId, ScopeId scopeId, CancellationToken cancellation = default)
    //     where TReadModel : class, new();
    //
    // /// <summary>
    // /// Gets a projection state by key for a projection specified by projection and scope identifier.
    // /// </summary>
    // /// <param name="key">The <see cref="Key" /> of the projection.</param>
    // /// <param name="projectionId">The <see cref="ProjectionId"/>.</param>
    // /// <param name="scopeId">The <see cref="ScopeId"/>.</param>
    // /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    // /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <see cref="CurrentState{TReadModel}"/> of <see cref="object"/>.</returns>
    // Task<CurrentState<object>> GetState(Key key, ProjectionId projectionId, ScopeId scopeId, CancellationToken cancellation = default);


    /// <summary>
    /// Gets all projection read models for a projection associated with a type.
    /// </summary>
    /// <typeparam name="TProjection">The <see cref="Type" /> of the projection.</typeparam>
    /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <see cref="IEnumerable{T}" /> of <typeparamref name="TProjection" />.</returns>
    IQueryable<TProjection> AsQueryable<TProjection>() where TProjection : ProjectionBase, new();
    
    // /// <summary>
    // /// Gets all projection read models for a projection specified by projection identifier.
    // /// </summary>
    // /// <param name="projectionId">The <see cref="ProjectionId"/>.</param>
    // /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    // /// <typeparam name="TReadModel">The <see cref="Type" /> of the projection.</typeparam>
    // /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <see cref="IEnumerable{T}" /> of <typeparamref name="TReadModel" />.</returns>
    // Task<IEnumerable<TReadModel>> GetAll<TReadModel>(ProjectionId projectionId, CancellationToken cancellation = default)
    //     where TReadModel : class, new();
    //
    // /// <summary>
    // /// Gets all projection read models for a projection specified by projection identifier.
    // /// </summary>
    // /// <param name="projectionId">The <see cref="ProjectionId"/>.</param>
    // /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    // /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <see cref="IEnumerable{T}" /> of <see cref="object" />.</returns>
    // Task<IEnumerable<object>> GetAll(ProjectionId projectionId, CancellationToken cancellation = default);
    //
    // /// <summary>
    // /// Gets all projection read models for a projection specified by projection and scope identifier.
    // /// </summary>
    // /// <param name="projectionId">The <see cref="ProjectionId"/>.</param>
    // /// <param name="scopeId">The <see cref="ScopeId"/>.</param>
    // /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    // /// <typeparam name="TReadModel">The <see cref="Type" /> of the projection.</typeparam>
    // /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <see cref="IEnumerable{T}" /> of <typeparamref name="TReadModel" />.</returns>
    // Task<IEnumerable<TReadModel>> GetAll<TReadModel>(ProjectionId projectionId, ScopeId scopeId, CancellationToken cancellation = default)
    //     where TReadModel : class, new();
    //
    // /// <summary>
    // /// Gets all projection read models for a projection specified by projection and scope identifier.
    // /// </summary>
    // /// <param name="projectionId">The <see cref="ProjectionId"/>.</param>
    // /// <param name="scopeId">The <see cref="ScopeId"/>.</param>
    // /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    // /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <see cref="IEnumerable{T}" /> of <see cref="object" />.</returns>
    // Task<IEnumerable<object>> GetAll(ProjectionId projectionId, ScopeId scopeId, CancellationToken cancellation = default);
}

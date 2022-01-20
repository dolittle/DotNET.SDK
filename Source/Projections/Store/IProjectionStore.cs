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
    /// Gets a projection state by key for a projection associated with a type.
    /// </summary>
    /// <param name="key">THe <see cref="Key" /> of the projection.</param>
    /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    /// <typeparam name="TProjection">The <see cref="Type" /> of the projection.</typeparam>
    /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <see cref="CurrentState{TProjection}" />.</returns>
    Task<CurrentState<TProjection>> Get<TProjection>(Key key, CancellationToken cancellation = default)
        where TProjection : class, new();

    /// <summary>
    /// Gets a projection state by key for a projection specified by projection identifier.
    /// </summary>
    /// <param name="key">THe <see cref="Key" /> of the projection.</param>
    /// <param name="projectionId">The <see cref="ProjectionId"/>.</param>
    /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    /// <typeparam name="TProjection">The <see cref="Type" /> of the projection.</typeparam>
    /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <see cref="CurrentState{TProjection}" />.</returns>
    Task<CurrentState<TProjection>> Get<TProjection>(Key key, ProjectionId projectionId, CancellationToken cancellation = default)
        where TProjection : class, new();

    /// <summary>
    /// Gets a projection state by key for a projection specified by projection identifier.
    /// </summary>
    /// <param name="key">THe <see cref="Key" /> of the projection.</param>
    /// <param name="projectionId">The <see cref="ProjectionId"/>.</param>
    /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <see cref="CurrentState{Object}" />.</returns>
    Task<CurrentState<object>> Get(Key key, ProjectionId projectionId, CancellationToken cancellation = default);

    /// <summary>
    /// Gets a projection state by key for a projection specified by projection and scope identifier.
    /// </summary>
    /// <param name="key">THe <see cref="Key" /> of the projection.</param>
    /// <param name="projectionId">The <see cref="ProjectionId"/>.</param>
    /// <param name="scopeId">The <see cref="ScopeId"/>.</param>
    /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    /// <typeparam name="TProjection">The <see cref="Type" /> of the projection.</typeparam>
    /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <see cref="CurrentState{TProjection}" />.</returns>
    Task<CurrentState<TProjection>> Get<TProjection>(Key key, ProjectionId projectionId, ScopeId scopeId, CancellationToken cancellation = default)
        where TProjection : class, new();

    /// <summary>
    /// Gets a projection state by key for a projection specified by projection and scope identifier.
    /// </summary>
    /// <param name="key">THe <see cref="Key" /> of the projection.</param>
    /// <param name="projectionId">The <see cref="ProjectionId"/>.</param>
    /// <param name="scopeId">The <see cref="ScopeId"/>.</param>
    /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <see cref="CurrentState{Object}" />.</returns>
    Task<CurrentState<object>> Get(Key key, ProjectionId projectionId, ScopeId scopeId, CancellationToken cancellation = default);

    /// <summary>
    /// Gets all projection states for a projection associated with a type.
    /// </summary>
    /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    /// <typeparam name="TProjection">The <see cref="Type" /> of the projection.</typeparam>
    /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <see cref="IDictionary{Key, T}" /> of <see cref="CurrentState{TProjection}" />.</returns>
    Task<IDictionary<Key, CurrentState<TProjection>>> GetAll<TProjection>(CancellationToken cancellation = default)
        where TProjection : class, new();

    /// <summary>
    /// Gets all projection states for a projection specified by projection identifier.
    /// </summary>
    /// <param name="projectionId">The <see cref="ProjectionId"/>.</param>
    /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    /// <typeparam name="TProjection">The <see cref="Type" /> of the projection.</typeparam>
    /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <see cref="IDictionary{Key, T}" /> of <see cref="CurrentState{TProjection}" />.</returns>
    Task<IDictionary<Key, CurrentState<TProjection>>> GetAll<TProjection>(ProjectionId projectionId, CancellationToken cancellation = default)
        where TProjection : class, new();

    /// <summary>
    /// Gets all projection states for a projection specified by projection identifier.
    /// </summary>
    /// <param name="projectionId">The <see cref="ProjectionId"/>.</param>
    /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <see cref="IDictionary{Key, T}" /> of <see cref="CurrentState{Object}" />.</returns>
    Task<IDictionary<Key, CurrentState<object>>> GetAll(ProjectionId projectionId, CancellationToken cancellation = default);

    /// <summary>
    /// Gets all projection states for a projection specified by projection and scope identifier.
    /// </summary>
    /// <param name="projectionId">The <see cref="ProjectionId"/>.</param>
    /// <param name="scopeId">The <see cref="ScopeId"/>.</param>
    /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    /// <typeparam name="TProjection">The <see cref="Type" /> of the projection.</typeparam>
    /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <see cref="IDictionary{Key, T}" /> of <see cref="CurrentState{TProjection}" />.</returns>
    Task<IDictionary<Key, CurrentState<TProjection>>> GetAll<TProjection>(ProjectionId projectionId, ScopeId scopeId, CancellationToken cancellation = default)
        where TProjection : class, new();

    /// <summary>
    /// Gets all projection states for a projection specified by projection and scope identifier.
    /// </summary>
    /// <param name="projectionId">The <see cref="ProjectionId"/>.</param>
    /// <param name="scopeId">The <see cref="ScopeId"/>.</param>
    /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns the <see cref="IDictionary{Key, T}" /> of <see cref="CurrentState{Object}" />.</returns>
    Task<IDictionary<Key, CurrentState<object>>> GetAll(ProjectionId projectionId, ScopeId scopeId, CancellationToken cancellation = default);
}
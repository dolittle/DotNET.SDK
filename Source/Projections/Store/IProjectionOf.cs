// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections.Store;

/// <summary>
/// Defines a system that knows about a projection.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the projection read model.</typeparam>
public interface IProjectionOf<TReadModel> where TReadModel : ReadModel, new()
{
    /// <summary>
    /// Gets the <see cref="ProjectionId"/> identifier.
    /// </summary>
    ProjectionId Identifier { get; }

    /// <summary>
    /// Gets the <see cref="ScopeId"/>.
    /// </summary>
    ScopeId Scope { get; }

    /// <summary>
    /// Gets the projection read model by projection key.
    /// </summary>
    /// <param name="key">The <see cref="Key" /> of the projection.</param>
    /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns the <typeparamref name="TReadModel"/> read model.</returns>
    Task<TReadModel?> Get(Key key, CancellationToken cancellation = default);
    
    /// <summary>
    /// Gets the projection read model by id (Which will be the same as the projection key)
    /// </summary>
    /// <param name="id">The <see cref="Key" /> of the projection.</param>
    /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns the <typeparamref name="TReadModel"/> read model.</returns>
    Task<TReadModel?> Get(string id, CancellationToken cancellation = default);

    /// <summary>
    /// Gets the current and later versions of the projection.
    /// Requires the projection to implement <see cref="ICloneable"/> in order to not return the same instance.
    /// </summary>
    ISubscription<TP?> Subscribe<TP>(string id, CancellationToken cancellationToken) where TP: TReadModel, ICloneable;
    
    /// <summary>
    /// Get an <see cref="IQueryable{TReadModel}"/> for the projection read model.
    /// </summary>
    /// <returns></returns>
    IQueryable<TReadModel> AsQueryable();
    
    /// <summary>
    /// Get an <see cref="IQueryable{TReadModel}"/> for the projection read model.
    /// </summary>
    /// <returns></returns>
    IQueryable<TReadModel> Query();
}

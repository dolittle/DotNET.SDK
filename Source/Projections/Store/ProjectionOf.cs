// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections.Store;

/// <summary>
/// Represents an implementation of <see cref="IProjectionOf{TReadModel}"/>.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the projection read model.</typeparam>
public class ProjectionOf<TReadModel> : IProjectionOf<TReadModel>
    where TReadModel : class, new()
{
    readonly IProjectionStore _projectionStore;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionOf{TReadModel}"/> class.
    /// </summary>
    /// <param name="projectionStore">The <see cref="IProjectionStore"/>.</param>
    /// <param name="scopedProjectionId">The <see cref="ScopedProjectionId"/>.</param>
    public ProjectionOf(IProjectionStore projectionStore, ScopedProjectionId scopedProjectionId)
    {
        _projectionStore = projectionStore;
        (Identifier, Scope) = scopedProjectionId;
    }

    /// <inheritdoc />
    public ProjectionId Identifier { get; }
    
    /// <inheritdoc />
    public ScopeId Scope { get; }

    /// <inheritdoc />
    public Task<TReadModel> Get(Key key, CancellationToken cancellation = default)
        => _projectionStore.Get<TReadModel>(key, Identifier, Scope, cancellation);

    /// <inheritdoc />
    public Task<CurrentState<TReadModel>> GetState(Key key, CancellationToken cancellation = default)
        => _projectionStore.GetState<TReadModel>(key, Identifier, Scope,cancellation);

    /// <inheritdoc />
    public Task<IEnumerable<TReadModel>> GetAll(CancellationToken cancellation = default)
        => _projectionStore.GetAll<TReadModel>(Identifier, Scope,cancellation);
}

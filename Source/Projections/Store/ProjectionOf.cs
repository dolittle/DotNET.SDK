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
/// Represents an implementation of <see cref="IProjectionOf{TReadModel}"/>.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the projection read model.</typeparam>
public class ProjectionOf<TReadModel> : IProjectionOf<TReadModel>
    where TReadModel : ProjectionBase, new()
{
    readonly IProjectionStore _projectionStore;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionOf{TReadModel}"/> class.
    /// </summary>
    /// <param name="projectionStore">The <see cref="IProjectionStore"/>.</param>
    /// <param name="identifier">The <see cref="ProjectionModelId"/>.</param>
    public ProjectionOf(IProjectionStore projectionStore, ProjectionModelId identifier) : this(projectionStore, identifier.Id, identifier.Scope)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionOf{TReadModel}"/> class.
    /// </summary>
    /// <param name="projectionStore">The <see cref="IProjectionStore"/>.</param>
    /// <param name="identifier">The <see cref="ProjectionId"/>.</param>
    /// <param name="scope">The <see cref="ScopeId"/>.</param>
    public ProjectionOf(IProjectionStore projectionStore, ProjectionId identifier, ScopeId scope)
    {
        _projectionStore = projectionStore;
        Identifier = identifier;
        Scope = scope;
    }

    /// <inheritdoc />
    public ProjectionId Identifier { get; }
    
    /// <inheritdoc />
    public ScopeId Scope { get; }

    /// <inheritdoc />
    public Task<TReadModel?> Get(Key key, CancellationToken cancellation = default)
        => _projectionStore.Get<TReadModel>(key, Identifier, Scope, cancellation);

    public IQueryable<TReadModel> AsQueryable()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<CurrentState<TReadModel>> GetState(Key key, CancellationToken cancellation = default)
        => _projectionStore.GetState<TReadModel>(key, Identifier, Scope,cancellation);

    /// <inheritdoc />
    public Task<IEnumerable<TReadModel>> GetAll(CancellationToken cancellation = default)
        => _projectionStore.GetAll<TReadModel>(Identifier, Scope,cancellation);
}

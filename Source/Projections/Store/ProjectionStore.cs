// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Store;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Dolittle.SDK.Projections.Store;

/// <summary>
/// Represents an implementation of <see cref="IProjectionStore" />.
/// </summary>
public class ProjectionStore : IProjectionStore
{
    readonly IServiceProvider _providers;
    readonly IProjectionReadModelTypes _projectionAssociations;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionStore"/> class.
    /// </summary>
    /// <param name="providers"></param>
    /// <param name="projectionAssociations">The <see cref="IProjectionReadModelTypes" />.</param>
    public ProjectionStore(
        IServiceProvider providers,
        IProjectionReadModelTypes projectionAssociations)
    {
        _providers = providers;
        _projectionAssociations = projectionAssociations;
    }

    /// <inheritdoc />
    public IProjectionOf<TProjection> Of<TProjection>()
        where TProjection : ReadModel, new()
        => new ProjectionOf<TProjection>(_providers.GetRequiredService<IMongoCollection<TProjection>>(), _projectionAssociations.GetFor<TProjection>());

    /// <inheritdoc />
    public IHistoricalProjection<TProjection> Historical<TProjection>()
        where TProjection : ReadModel, new()
    {
        var projection = _providers.GetRequiredService<IProjection<TProjection>>();
        if (!projection.SupportsHistoricalQueries)
        {
            throw new ArgumentException(
                "Projection type does not support historical queries. It is available if the projection uses only events keyed on EventSourceId");
        }

        return new HistoricalProjection<TProjection>(projection, _providers.GetRequiredService<IEventStore>());
    }


    /// <inheritdoc />
    public IProjectionOf<TReadModel> Of<TReadModel>(ProjectionId projectionId)
        where TReadModel : ReadModel, new()
        => new ProjectionOf<TReadModel>(_providers.GetRequiredService<IMongoCollection<TReadModel>>(), projectionId, ScopeId.Default);

    /// <inheritdoc />
    public IProjectionOf<TReadModel> Of<TReadModel>(ProjectionId projectionId, ScopeId scopeId)
        where TReadModel : ReadModel, new()
        => new ProjectionOf<TReadModel>(_providers.GetRequiredService<IMongoCollection<TReadModel>>(), projectionId, scopeId);

    /// <inheritdoc/>
    public Task<TProjection?> Get<TProjection>(Key key, CancellationToken cancellation = default)
        where TProjection : ReadModel, new()
    {
        return Of<TProjection>().Get(key, cancellation);
    }

    /// <inheritdoc/>
    public IQueryable<TProjection> AsQueryable<TProjection>() where TProjection : ReadModel, new()
        => Of<TProjection>().AsQueryable();

    /// <inheritdoc/>
    public Task<TReadModel?> Get<TReadModel>(Key key, ProjectionId projectionId, CancellationToken cancellation = default)
        where TReadModel : ReadModel, new()
        => Of<TReadModel>(projectionId).Get(key, cancellation);

    /// <inheritdoc/>
    public Task<TReadModel?> Get<TReadModel>(Key key, ProjectionId projectionId, ScopeId scopeId, CancellationToken cancellation = default)
        where TReadModel : ReadModel, new()
    {
        return Of<TReadModel>(projectionId, scopeId).Get(key, cancellation);
    }
}

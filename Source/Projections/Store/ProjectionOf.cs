// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Events;
using MongoDB.Driver;

namespace Dolittle.SDK.Projections.Store;

/// <summary>
/// Represents an implementation of <see cref="IProjectionOf{TReadModel}"/>.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the projection read model.</typeparam>
public class ProjectionOf<TReadModel> : IProjectionOf<TReadModel>
    where TReadModel : ReadModel, new()
{
    readonly IMongoCollection<TReadModel> _collection;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionOf{TReadModel}"/> class.
    /// </summary>
    /// <param name="collection">The <see cref="IMongoCollection{TReadModel}"/> (tenanted).</param>
    /// <param name="identifier">The <see cref="ProjectionModelId"/>.</param>
    public ProjectionOf(IMongoCollection<TReadModel> collection, ProjectionModelId identifier) : this(collection, identifier.Id, identifier.Scope)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionOf{TReadModel}"/> class.
    /// </summary>
    /// <param name="collection">The <see cref="IMongoCollection{TReadModel}"/> (tenanted).</param>
    /// <param name="identifier">The <see cref="ProjectionId"/>.</param>
    /// <param name="scope">The <see cref="ScopeId"/>.</param>
    public ProjectionOf(IMongoCollection<TReadModel> collection, ProjectionId identifier, ScopeId scope)
    {
        _collection = collection;
        Identifier = identifier;
        Scope = scope;
    }

    /// <inheritdoc />
    public ProjectionId Identifier { get; }

    /// <inheritdoc />
    public ScopeId Scope { get; }

    public Task<TReadModel?> Get(Key key, CancellationToken cancellation = default) => Get(key.Value, cancellation);

    /// <inheritdoc />
    public Task<TReadModel?> Get(string id, CancellationToken cancellation = default)
        => _collection.Find(it => it.Id.Equals(id)).FirstOrDefaultAsync(cancellationToken: cancellation)!;

    /// <inheritdoc />
    public IQueryable<TReadModel> AsQueryable() => _collection.AsQueryable();

    /// <inheritdoc />
    public IQueryable<TReadModel> Query() => _collection.AsQueryable();
}

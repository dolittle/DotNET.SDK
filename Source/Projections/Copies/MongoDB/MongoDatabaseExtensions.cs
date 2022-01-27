// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using MongoDB.Driver;

namespace Dolittle.SDK.Projections.Copies.MongoDB;

/// <summary>
/// Extensions for <see cref="IMongoDatabase"/> related to projection MongoDB copies.
/// </summary>
public static class MongoDatabaseExtensions
{
    /// <summary>
    /// Gets the <see cref="IMongoCollection{TDocument}"/> for <typeparamref name="TProjection"/>.
    /// </summary>
    /// <param name="database">The <see cref="IMongoDatabase"/>.</param>
    /// <param name="settings">The optional <see cref="MongoCollectionSettings"/> used when getting the <see cref="IMongoCollection{TDocument}"/>.</param>
    /// <typeparam name="TProjection">The <see cref="Type"/> of the projection read model. </typeparam>
    /// <returns>The <see cref="IMongoCollection{TDocument}"/> for <typeparamref name="TProjection"/>.</returns>
    /// <exception cref="CannotGetProjectionMongoDBCopyCollectionNameFromType">Exception that is thrown when the <see cref="ProjectionMongoDBCopyCollectionName"/> cannot be retrieved from the <typeparamref name="TProjection"/> <see cref="Type"/>.</exception>
    public static IMongoCollection<TProjection> GetCollection<TProjection>(this IMongoDatabase database, MongoCollectionSettings settings = default)
        where TProjection : class, new()
    {
        if (!ProjectionMongoDBCopyCollectionName.TryGetFrom<TProjection>(out var collectionName))
        {
            throw new CannotGetProjectionMongoDBCopyCollectionNameFromType(typeof(TProjection));
        }
        return database.GetCollection<TProjection>(collectionName, settings);
    }
}

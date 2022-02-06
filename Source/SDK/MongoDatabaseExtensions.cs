// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using Dolittle.SDK.Projections.Copies.MongoDB;
using MongoDB.Driver;

namespace Dolittle.SDK;

/// <summary>
/// Extensions for <see cref="IMongoDatabase"/>.
/// </summary>
public static class MongoDatabaseExtensions
{
    /// <summary>
    /// Gets the <see cref="IMongoCollection{TDocument}"/> for <typeparamref name="TSchema"/> where
    /// either the <see cref="CopyProjectionToMongoDBAttribute.CollectionName"/> or the <see cref="Type.Name"/> will be used as the .
    /// </summary>
    /// <param name="database">The <see cref="IMongoDatabase"/>.</param>
    /// <param name="settings">The optional <see cref="MongoCollectionSettings"/> used when getting the <see cref="IMongoCollection{TDocument}"/>.</param>
    /// <typeparam name="TSchema">The <see cref="Type"/> of the projection read model. </typeparam>
    /// <returns>The <see cref="IMongoCollection{TDocument}"/> for <typeparamref name="TSchema"/>.</returns>
    public static IMongoCollection<TSchema> GetCollection<TSchema>(this IMongoDatabase database, MongoCollectionSettings settings = default)
        where TSchema : class
        => database.GetCollection<TSchema>(MongoDBCopyCollectionName.GetFrom<TSchema>(), settings);
}

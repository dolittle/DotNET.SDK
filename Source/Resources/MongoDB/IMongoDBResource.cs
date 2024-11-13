// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using MongoDB.Driver;

namespace Dolittle.SDK.Resources.MongoDB;

/// <summary>
/// Defines the MongoDB resource.
/// </summary>
public interface IMongoDBResource
{
    /// <summary>
    /// Gets the <see cref="IMongoDatabase"/> that is configured for this <see cref="IMongoDBResource"/>.
    /// </summary>
    /// <param name="databaseSettingsCallback">The <see cref="Action{T}"/> callback for creating <see cref="MongoDatabaseSettings"/> used to create the <see cref="IMongoDatabase"/>.</param>
    /// <returns>An <see cref="IMongoDatabase"/>.</returns>
    IMongoDatabase GetDatabase(Action<MongoDatabaseSettings>? databaseSettingsCallback = default);
    
    /// <summary>
    /// Gets the connection string that is used to connect to the MongoDB resource.
    /// </summary>
    string ConnectionString { get; }
    
    /// <summary>
    /// Gets the <see cref="MongoUrl"/> that is used to connect to the MongoDB resource.
    /// </summary>
    MongoUrl ConnectionUrl { get; }
}

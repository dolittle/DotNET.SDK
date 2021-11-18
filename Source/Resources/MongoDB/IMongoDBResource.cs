// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Dolittle.SDK.Resources.MongoDB
{
    /// <summary>
    /// Defines the MongoDB <see cref="IResource"/>.
    /// </summary>
    public interface IMongoDBResource : IResource
    {
        /// <summary>
        /// Gets the <see cref="IMongoDatabase"/> that is configured for this <see cref="IResource"/>.
        /// </summary>
        /// <param name="databaseSettingsCallback">The <see cref="Action{T}"/> callback for creating <see cref="MongoDatabaseSettings"/> used to create the <see cref="IMongoDatabase"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task{TResult}"/>that, when resolved, returns the <see cref="IMongoDatabase"/>.</returns>
        Task<IMongoDatabase> GetDatabase(Action<MongoDatabaseSettings> databaseSettingsCallback = default, CancellationToken cancellationToken = default);
    }
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Tenancy;
using MongoDB.Driver;

namespace Dolittle.SDK.Resources
{
    /// <summary>
    /// Defines a system that knows about Resources provided by the Runtime.
    /// </summary>
    public interface IResources
    {
        /// <summary>
        /// Gets the MongoDB resource for a tenant.
        /// </summary>
        /// <param name="tenantId">The <see cref="TenantId"/> to get the resource for.</param>
        /// <param name="databaseSettings">The optional <see cref="MongoDatabaseSettings"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="FailedToGetResource">Exception that gets thrown when the Runtime failed getting the resource.</exception>
        Task<IMongoDatabase> GetMongoDB(TenantId tenantId, MongoDatabaseSettings databaseSettings = default, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the MongoDB resource for a tenant.
        /// </summary>
        /// <param name="tenantId">The <see cref="TenantId"/> to get the resource for.</param>
        /// <param name="createClientFromConnectionString">The <see cref="Func{T1, TResult}"/>that creates the <see cref="MongoClient"/> from a connection string.</param>
        /// <param name="databaseSettings">The optional <see cref="MongoDatabaseSettings"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="FailedToGetResource">Exception that gets thrown when the Runtime failed getting the resource.</exception>
        Task<IMongoDatabase> GetMongoDB(TenantId tenantId, Func<string, MongoClient> createClientFromConnectionString, MongoDatabaseSettings databaseSettings = default,CancellationToken cancellationToken = default);
    }
}
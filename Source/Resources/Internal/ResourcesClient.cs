// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Protobuf;
using Dolittle.SDK.Services;
using Dolittle.SDK.Tenancy;
using Dolittle.Services.Contracts;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Contracts = Dolittle.Runtime.Resources.Contracts;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.Resources.Internal
{
    /// <summary>
    /// Represents a client for <see cref="Contracts.Resources"/> and an implementation of <see cref="IResources"/>.
    /// </summary>
    public class ResourcesClient : IResources
    {
        static readonly ResourcesGetMongoDbMethod _method = new ResourcesGetMongoDbMethod();
        readonly IPerformMethodCalls _caller;
        readonly ExecutionContext _executionContext;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourcesClient"/> class.
        /// </summary>
        /// <param name="caller">The method caller to use to perform calls to the Runtime.</param>
        /// <param name="executionContext">The <see cref="ExecutionContext"/>.</param>
        /// <param name="logger">The <see cref="ILogger"/> to use.</param>
        public ResourcesClient(IPerformMethodCalls caller, ExecutionContext executionContext, ILogger logger)
        {
            _caller = caller;
            _executionContext = executionContext;
            _logger = logger;
        }

        /// <inheritdoc />
        public Task<IMongoDatabase> GetMongoDB(TenantId tenantId, MongoDatabaseSettings databaseSettings = default, CancellationToken cancellationToken = default)
            => GetMongoDB(
                tenantId,
                connectionString => new MongoClient(MongoClientSettings.FromConnectionString(connectionString).Freeze()),
                databaseSettings,
                cancellationToken);

        /// <inheritdoc />
        public async Task<IMongoDatabase> GetMongoDB(TenantId tenantId, Func<string, MongoClient> createClientFromConnectionString, MongoDatabaseSettings databaseSettings = default, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Getting MongoDB resource for {Tenant}", tenantId.Value);
            try
            {
                var request = new Contracts.GetRequest { CallContext = new CallRequestContext { ExecutionContext = _executionContext.ForTenant(tenantId).ToProtobuf() } };
                var response = await _caller.Call(_method, request, cancellationToken).ConfigureAwait(false);
                if (response.Failure == null)
                {
                    var client = createClientFromConnectionString(response.ConnectionString);
                    return client.GetDatabase(response.DatabaseName, databaseSettings);
                }

                _logger.LogWarning("An error occurred while getting all Tenants because {Reason}. Failure Id '{FailureId}'", response.Failure.Reason, response.Failure.Id.ToGuid().ToString());
                throw new FailedToGetResource("MongoDB", tenantId, response.Failure.Reason);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "An error occurred while getting all Tenants");
                throw;
            }
        }
    }
}
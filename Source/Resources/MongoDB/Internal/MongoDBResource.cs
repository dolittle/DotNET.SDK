// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Protobuf.Contracts;
using Dolittle.SDK.Services;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;
using MongoDBBaseResource = Dolittle.SDK.Resources.Resource<Dolittle.Runtime.Resources.Contracts.GetRequest, Dolittle.Runtime.Resources.Contracts.GetMongoDBResponse>;

namespace Dolittle.SDK.Resources.MongoDB.Internal;

/// <summary>
/// Represents a client for <see cref="Contracts.Resources"/> and an implementation of <see cref="IResources"/>.
/// </summary>
public class MongoDBResource : MongoDBBaseResource, IMongoDBResource
{
    static readonly ResourcesGetMongoDBMethod _method = new();
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDBResource"/> class.
    /// </summary>
    /// <param name="tenant">The <see cref="TenantId"/>.</param>
    /// <param name="caller">The method caller to use to perform calls to the Runtime.</param>
    /// <param name="executionContext">The <see cref="ExecutionContext"/>.</param>
    /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
    public MongoDBResource(TenantId tenant, IPerformMethodCalls caller, ExecutionContext executionContext, ILoggerFactory loggerFactory)
        : base("MongoDB", tenant, caller, executionContext, loggerFactory.CreateLogger<MongoDBBaseResource>() )
    {
        _logger = loggerFactory.CreateLogger<MongoDBResource>();
    }

    /// <inheritdoc />
    public async Task<IMongoDatabase> GetDatabase(Action<MongoDatabaseSettings> databaseSettingsCallback = default, CancellationToken cancellationToken = default)
    {
        var connectionString = await Get(_method, response => response.ConnectionString, cancellationToken).ConfigureAwait(false);
        var databaseSettings = new MongoDatabaseSettings();
        databaseSettingsCallback?.Invoke(databaseSettings);
        var mongoUrl = MongoUrl.Create(connectionString);
        var clientSettings = MongoClientSettings.FromUrl(mongoUrl);
        return new MongoClient(clientSettings.Freeze()).GetDatabase(mongoUrl.DatabaseName, databaseSettings);
    }

    /// <inheritdoc />
    protected override Runtime.Resources.Contracts.GetRequest CreateRequest()
        => new() { CallContext = CreateRequestContext() };

    /// <inheritdoc />
    protected override bool TryGetFailureFromResponse(Runtime.Resources.Contracts.GetMongoDBResponse response, out Failure failure)
    {
        failure = response.Failure;
        return failure != default;
    }
}

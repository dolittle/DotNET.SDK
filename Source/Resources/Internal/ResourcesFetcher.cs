// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Resources.MongoDB.Internal;
using Dolittle.SDK.Services;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.Resources.Internal;

/// <summary>
/// Represents an implementation of <see cref="IFetchResources"/>.
/// </summary>
public class ResourcesFetcher : IFetchResources
{
    readonly MongoDBResourceCreator _mongoDB;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ResourcesFetcher"/> class.
    /// </summary>
    /// <param name="methodCaller">The method caller to make requests to the Runtime with.</param>
    /// <param name="executionContext">The base execution context for the client.</param>
    /// <param name="loggerFactory">The logger factory to use to create loggers.</param>
    public ResourcesFetcher(IPerformMethodCalls methodCaller, ExecutionContext executionContext, ILoggerFactory loggerFactory)
    {
        _mongoDB = new MongoDBResourceCreator(methodCaller, executionContext, loggerFactory);
    }

    /// <inheritdoc />
    public async Task<IResourcesBuilder> FetchResourcesFor(IEnumerable<Tenant> tenants, CancellationToken cancellationToken = default)
    {
        var resources = new ConcurrentDictionary<TenantId, IResources>();

        await Task.WhenAll(tenants.Select(async tenant =>
        {
            resources.TryAdd(
                tenant.Id,
                new Resources(
                    await _mongoDB.CreateFor(tenant.Id, cancellationToken).ConfigureAwait(false)
                )
            );
        })).ConfigureAwait(false);
            
        return new ResourcesBuilder(resources);
    }
}

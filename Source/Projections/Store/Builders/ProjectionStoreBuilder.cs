// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Projections.Store.Builders;

/// <summary>
/// Represents an implementation of <see cref="IProjectionStoreBuilder"/>.
/// </summary>
public class ProjectionStoreBuilder : IProjectionStoreBuilder
{
    readonly Func<TenantId, IServiceProvider> _getServiceProvider;
    readonly ExecutionContext _executionContext;
    readonly IProjectionReadModelTypes _projectionAssociations;
    readonly ILoggerFactory _loggerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionStoreBuilder"/> class.
    /// </summary>
    /// <param name="getServiceProvider"></param>
    /// <param name="executionContext">The base <see cref="ExecutionContext"/> to use.</param>
    /// <param name="projectionAssociations">The <see cref="IProjectionReadModelTypes" />.</param>
    /// <param name="projectionsToSDKConverter">The <see cref="IConvertProjectionsToSDK" />.</param>
    /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
    public ProjectionStoreBuilder(
        Func<TenantId, IServiceProvider> getServiceProvider,
        ExecutionContext executionContext,
        IProjectionReadModelTypes projectionAssociations,
        ILoggerFactory loggerFactory)
    {
        _getServiceProvider = getServiceProvider;
        _executionContext = executionContext;
        _projectionAssociations = projectionAssociations;
        _loggerFactory = loggerFactory;
    }

    /// <inheritdoc />
    public IProjectionStore ForTenant(TenantId tenantId)
    {
        var provider = _getServiceProvider(tenantId);
        
        var executionContext = _executionContext
            .ForTenant(tenantId)
            .ForCorrelation(Guid.NewGuid());

        
        return new ProjectionStore(
            provider,
            executionContext,
            _projectionAssociations,
            _loggerFactory.CreateLogger<ProjectionStore>());
    }
}

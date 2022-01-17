// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Projections.Store.Converters;
using Dolittle.SDK.Services;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Projections.Store.Builders;

/// <summary>
/// Represents an implementation of <see cref="IProjectionStoreBuilder"/>.
/// </summary>
public class ProjectionStoreBuilder : IProjectionStoreBuilder
{
    readonly IPerformMethodCalls _caller;
    readonly ExecutionContext _executionContext;
    readonly IResolveCallContext _callContextResolver;
    readonly IProjectionReadModelTypes _projectionAssociations;
    readonly IConvertProjectionsToSDK _projectionsToSDKConverter;
    readonly ILoggerFactory _loggerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionStoreBuilder"/> class.
    /// </summary>
    /// <param name="caller">The caller for unary calls.</param>
    /// <param name="executionContext">The base <see cref="ExecutionContext"/> to use.</param>
    /// <param name="callContextResolver">The <see cref="IResolveCallContext" />.</param>
    /// <param name="projectionAssociations">The <see cref="IProjectionReadModelTypes" />.</param>
    /// <param name="projectionsToSDKConverter">The <see cref="IConvertProjectionsToSDK" />.</param>
    /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
    public ProjectionStoreBuilder(
        IPerformMethodCalls caller,
        ExecutionContext executionContext,
        IResolveCallContext callContextResolver,
        IProjectionReadModelTypes projectionAssociations,
        IConvertProjectionsToSDK projectionsToSDKConverter,
        ILoggerFactory loggerFactory)
    {
        _caller = caller;
        _executionContext = executionContext;
        _callContextResolver = callContextResolver;
        _projectionAssociations = projectionAssociations;
        _projectionsToSDKConverter = projectionsToSDKConverter;
        _loggerFactory = loggerFactory;
    }

    /// <inheritdoc />
    public IProjectionStore ForTenant(TenantId tenantId)
    {
        var executionContext = _executionContext
            .ForTenant(tenantId)
            .ForCorrelation(Guid.NewGuid());

        return new ProjectionStore(
            _caller,
            _callContextResolver,
            executionContext,
            _projectionAssociations,
            _projectionsToSDKConverter,
            _loggerFactory.CreateLogger<ProjectionStore>());
    }
}
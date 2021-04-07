// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Projections.Store.Converters;
using Dolittle.SDK.Services;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Projections.Store.Builders
{
    /// <summary>
    /// Represents a builder for building <see cref="Projections"/>.
    /// </summary>
    public class ProjectionStoreBuilder
    {
        readonly IPerformMethodCalls _caller;
        readonly ExecutionContext _executionContext;
        readonly IResolveCallContext _callContextResolver;
        readonly IProjectionAssociations _projectionAssociations;
        readonly IConvertProjectionsToSDK _projectionsToSDKConverter;
        readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionsBuilder"/> class.
        /// </summary>
        /// <param name="caller">The caller for unary calls.</param>
        /// <param name="executionContext">The base <see cref="ExecutionContext"/> to use.</param>
        /// <param name="callContextResolver">The <see cref="IResolveCallContext" />.</param>
        /// <param name="projectionAssociations">The <see cref="IProjectionAssociations" />.</param>
        /// <param name="projectionsToSDKConverter">The <see cref="IConvertProjectionsToSDK" />.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
        public ProjectionsBuilder(
            IPerformMethodCalls caller,
            ExecutionContext executionContext,
            IResolveCallContext callContextResolver,
            IProjectionAssociations projectionAssociations,
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

        /// <summary>
        /// Gets the projection store <see cref="IProjections"/> for the given tenant.
        /// </summary>
        /// <param name="tenantId">The <see cref="TenantId">tenant</see> to get projections for.</param>
        /// <returns>An <see cref="IProjections"/>.</returns>
        public IProjections ForTenant(TenantId tenantId)
        {
            var executionContext = _executionContext
                .ForTenant(tenantId)
                .ForCorrelation(Guid.NewGuid());

            return new Projections(
                _caller,
                _callContextResolver,
                executionContext,
                _projectionAssociations,
                _projectionsToSDKConverter,
                _loggerFactory.CreateLogger<Projections>());
        }
    }
}

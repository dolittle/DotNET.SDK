// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Resources.MongoDB.Internal;
using Dolittle.SDK.Services;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Resources
{
    /// <summary>
    /// Represents an implementation of <see cref="IResourcesBuilder"/>.
    /// </summary>
    public class ResourcesBuilder : IResourcesBuilder
    {
        readonly IPerformMethodCalls _methodCaller;
        readonly ExecutionContext _executionContext;
        readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourcesBuilder"/> class.
        /// </summary>
        /// <param name="methodCaller">The <see cref="IPerformMethodCalls"/>.</param>
        /// <param name="executionContext">The <see cref="ExecutionContext"/>.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
        public ResourcesBuilder(IPerformMethodCalls methodCaller, ExecutionContext executionContext, ILoggerFactory loggerFactory)
        {
            _methodCaller = methodCaller;
            _executionContext = executionContext;
            _loggerFactory = loggerFactory;
        }

        /// <inheritdoc />
        public IResources ForTenant(TenantId tenant)
        {
            var executionContext = _executionContext.ForTenant(tenant).ForCorrelation(Guid.NewGuid());
            return new Resources(new MongoDBResource(tenant, _methodCaller, executionContext, _loggerFactory));
        }
    }
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Tenancy.Contracts;
using Dolittle.SDK.Protobuf;
using Dolittle.SDK.Services;
using Microsoft.Extensions.Logging;
using Contracts = Dolittle.Runtime.Tenancy.Contracts;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.Tenancy.Internal
{
    /// <summary>
    /// Represents a client for <see cref="Contracts.Tenants"/> and an implementation of <see cref="ITenants"/>.
    /// </summary>
    public class TenantsClient : ITenants
    {
        static readonly TenantsGetAllMethod _method = new TenantsGetAllMethod();
        readonly IPerformMethodCalls _caller;
        readonly ExecutionContext _executionContext;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantsClient"/> class.
        /// </summary>
        /// <param name="caller">The method caller to use to perform calls to the Runtime.</param>
        /// <param name="executionContext">Tha base <see cref="ExecutionContext"/>.</param>
        /// <param name="logger">The <see cref="ILogger"/> to use.</param>
        public TenantsClient(IPerformMethodCalls caller, ExecutionContext executionContext, ILogger logger)
        {
            _caller = caller;
            _executionContext = executionContext;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Tenant>> GetAll(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Getting all Tenants");
            try
            {
                var response = await _caller.Call(_method, new GetAllRequest(), cancellationToken).ConfigureAwait(false);
                if (response.Failure == null)
                {
                    return response.Tenants.Select(CreateTenant);
                }

                _logger.LogWarning("An error occurred while getting all Tenants because {Reason}. Failure Id '{FailureId}'", response.Failure.Reason, response.Failure.Id.ToGuid().ToString());
                throw new FailedToGetAllTenants(response.Failure.Reason);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "An error occurred while getting all Tenants");
                throw;
            }
        }

        static Tenant CreateTenant(Contracts.Tenant tenant)
            => new Tenant(tenant.Id.ToGuid());
    }
}
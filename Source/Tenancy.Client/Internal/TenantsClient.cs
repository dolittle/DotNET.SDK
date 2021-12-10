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
using Dolittle.Services.Contracts;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.Tenancy.Client.Internal;

/// <summary>
/// Represents a client for <see cref="Runtime.Tenancy.Contracts.Tenants"/> and an implementation of <see cref="ITenants"/>.
/// </summary>
public class TenantsClient : ITenants
{
    static readonly TenantsGetAllMethod _method = new();
    readonly IPerformMethodCalls _caller;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TenantsClient"/> class.
    /// </summary>
    /// <param name="caller">The method caller to use to perform calls to the Runtime.</param>
    /// <param name="logger">The <see cref="ILogger"/> to use.</param>
    public TenantsClient(IPerformMethodCalls caller, ILogger logger)
    {
        _caller = caller;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Tenant>> GetAll(ExecutionContext executionContext, CancellationToken cancellationToken = default)
    {
        Log.GettingAllTenants(_logger);
        try
        {
            var request = new GetAllRequest { CallContext = new CallRequestContext { ExecutionContext = executionContext.ToProtobuf() } };
            var response = await _caller.Call(_method, request, cancellationToken).ConfigureAwait(false);
            if (response.Failure == null)
            {
                return response.Tenants.Select(CreateTenant);
            }

            Log.FailedGettingAllTenants(_logger, response.Failure.Reason, response.Failure.Id.ToGuid());
            throw new FailedToGetAllTenants(response.Failure.Reason);
        }
        catch (Exception ex)
        {
            Log.ErrorGettingTenants(_logger, ex);
            throw;
        }
    }

    static Tenant CreateTenant(Runtime.Tenancy.Contracts.Tenant tenant)
        => new(tenant.Id.ToGuid());
}

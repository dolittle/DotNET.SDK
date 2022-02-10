// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Protobuf.Contracts;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Protobuf;
using Dolittle.SDK.Services;
using Dolittle.SDK.Tenancy;
using Dolittle.Services.Contracts;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.Resources.Internal;

/// <summary>
/// Represents a system that can create a resource by fetching configuration from the Runtime.
/// </summary>
public abstract class ResourceCreator<TResource, TRequest, TResponse>
    where TRequest : IMessage
    where TResponse : IMessage
{
    readonly ResourceName _resource;
    readonly ICanCallAUnaryMethod<TRequest, TResponse> _method;
    readonly IPerformMethodCalls _methodCaller;
    readonly ExecutionContext _executionContext;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceCreator{TResource,TRequest,TResponse}"/> class.
    /// </summary>
    /// <param name="resource">The name of the resource type.</param>
    /// <param name="method">The gRPC method to call to get the resource configuration from the Runtime.</param>
    /// <param name="methodCaller">The method caller to make requests to the Runtime with.</param>
    /// <param name="executionContext">The base execution context for the client.</param>
    /// <param name="loggerFactory">The logger factory to use to create loggers.</param>
    protected ResourceCreator(ResourceName resource, ICanCallAUnaryMethod<TRequest, TResponse> method, IPerformMethodCalls methodCaller, ExecutionContext executionContext, ILoggerFactory loggerFactory)
    {
        _resource = resource;
        _method = method;
        _methodCaller = methodCaller;
        _executionContext = executionContext;
        _logger = loggerFactory.CreateLogger<ResourceCreator<TResource, TRequest, TResponse>>();
    }

    /// <summary>
    /// Creates the resource for the provided tenant by fetching configuration from the Runtime.
    /// </summary>
    /// <param name="tenant">The tenant id to create the resource for.</param>
    /// <param name="cancellationToken">An optional cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> that, when resolved, returns the created resource.</returns>
    public async Task<TResource> CreateFor(TenantId tenant, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting {ResourceName} resource for tenant {Tenant}", _resource, tenant);
            
            var executionContext = _executionContext.ForTenant(tenant);
            var callContext = new CallRequestContext {ExecutionContext = executionContext.ToProtobuf()};

            var request = CreateResourceRequest(callContext);
            var response = await _methodCaller.Call(_method, request, cancellationToken).ConfigureAwait(false);

            if (RequestFailed(response, out var failure))
            {
                _logger.LogWarning("Failed getting {ResourceName} resource for tenant {Tenant} because {Reason}", _resource, tenant, failure.Reason);
                throw new FailedToGetResourceForTenant(_resource, tenant, failure.Reason);
            }

            return CreateResourceFrom(response);
        }
        catch (Exception ex) when (ex is not FailedToGetResourceForTenant)
        {
            _logger.LogWarning(ex, "Failed getting {ResourceName} resource for tenant {Tenant} because {Reason}", _resource, tenant);
            throw new FailedToGetResourceForTenant(_resource, tenant, ex.Message);
        }
    }

    /// <summary>
    /// Creates a request to get the resource configuration using the provided call context.
    /// </summary>
    /// <param name="callContext">The call context to use for the request containing the tenant id.</param>
    /// <returns>A <typeparamref name="TRequest"/>.</returns>
    protected abstract TRequest CreateResourceRequest(CallRequestContext callContext);

    /// <summary>
    /// Checks whether or not the request failed based on the response.
    /// </summary>
    /// <param name="response">The response received from the Runtime.</param>
    /// <param name="failure">If failed, the failure received from the Runtime.</param>
    /// <returns>True if the request failed, false if not.</returns>
    protected abstract bool RequestFailed(TResponse response, out Failure failure);

    /// <summary>
    /// Creates the resource from the configuration received from the Runtime.
    /// </summary>
    /// <param name="response">The response received from the Runtime.</param>
    /// <returns>The created <typeparamref name="TResource"/>.</returns>
    protected abstract TResource CreateResourceFrom(TResponse response);
}

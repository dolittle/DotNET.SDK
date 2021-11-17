// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Protobuf.Contracts;
using Dolittle.SDK.Protobuf;
using Dolittle.SDK.Services;
using Dolittle.SDK.Tenancy;
using Dolittle.Services.Contracts;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.Resources
{
    /// <summary>
    /// Defines the base implementation of an <see cref="IResource"/>.
    /// </summary>
    /// <typeparam name="TRequest">The <see cref="Type"/> of the request message.</typeparam>
    /// <typeparam name="TResponse">The <see cref="Type"/> of the response message.</typeparam>
    public abstract class Resource<TRequest, TResponse> : IResource
        where TRequest : IMessage
        where TResponse : IMessage
    {
        readonly IPerformMethodCalls _methodCaller;
        readonly ExecutionContext _executionContext;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Resource{TRequest, TResponse}"/> class.
        /// </summary>
        /// <param name="name">The <see cref="ResourceName"/>.</param>
        /// <param name="tenant">The <see cref="TenantId"/>.</param>
        /// <param name="methodCaller">The <see cref="IPerformMethodCalls"/>.</param>
        /// <param name="executionContext">The <see cref="ExecutionContext"/>.</param>
        /// <param name="logger">The <see cref="ILogger"/>.</param>
        protected Resource(ResourceName name, TenantId tenant, IPerformMethodCalls methodCaller, ExecutionContext executionContext, ILogger logger)
        {
            _methodCaller = methodCaller;
            _executionContext = executionContext;
            _logger = logger;
            Name = name;
            Tenant = tenant;
        }

        /// <inheritdoc />
        public ResourceName Name { get; }

        /// <inheritdoc />
        public TenantId Tenant { get; }

        /// <summary>
        /// Gets the resource.
        /// </summary>
        /// <param name="method">The method to call.</param>
        /// <param name="getResult">The callback for retrieving the result form the response.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <typeparam name="TResult">The type of the resource.</typeparam>
        /// <returns> A <see cref="Task{TResult}"/>that, when resolved, returns the <typeparamref name="TResult"/> result.</returns>
        /// <exception cref="FailedToGetResource">The exception that gets thrown when failed to get the resource.</exception>
        protected async Task<TResult> Get<TResult>(ICanCallAUnaryMethod<TRequest, TResponse> method, Func<TResponse, TResult> getResult, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Getting {ResourceName} resource for tenant {Tenant}", this.Name.Value, this.Tenant.Value.ToString());
                var response = await _methodCaller.Call(method, CreateRequest(), cancellationToken).ConfigureAwait(false);
                if (!TryGetFailureFromResponse(response, out var failure))
                {
                    return getResult(response);
                }

                _logger.LogWarning("Failed getting {ResourceName} resource for tenant {Tenant} because {Reason}", this.Name.Value, this.Tenant.Value.ToString(), failure.Reason);
                throw new FailedToGetResource(this, failure.Reason);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed getting {ResourceName} resource for tenant {Tenant}", this.Name.Value, this.Tenant.Value.ToString());
                throw new FailedToGetResource(this, ex.Message);
            }
        }

        /// <summary>
        /// Creates the <see cref="CallRequestContext"/> from the <see cref="ExecutionContext"/>.
        /// </summary>
        /// <returns>The created <see cref="CallRequestContext"/>.</returns>
        protected CallRequestContext CreateRequestContext()
            => new CallRequestContext { ExecutionContext = _executionContext.ToProtobuf() };

        /// <summary>
        /// Creates the <typeparamref name="TRequest"/>.
        /// </summary>
        /// <returns>The <typeparamref name="TRequest"/>.</returns>
        protected abstract TRequest CreateRequest();

        /// <summary>
        /// Tries to get the <see cref="Failure"/> from the <typeparamref name="TResponse"/>.
        /// </summary>
        /// <param name="response">The <typeparamref name="TResponse"/>.</param>
        /// <param name="failure">The output <see cref="Failure"/>.</param>
        /// <returns>A value indicating whether the <typeparamref name="TResponse"/> has a failure.</returns>
        protected abstract bool TryGetFailureFromResponse(TResponse response, out Failure failure);
    }
}
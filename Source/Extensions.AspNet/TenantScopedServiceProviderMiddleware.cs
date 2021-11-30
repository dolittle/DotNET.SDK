// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.SDK.Async;
using Dolittle.SDK.Tenancy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Extensions.AspNet
{
    /// <summary>
    /// Represents a middleware that intercepts <see cref="HttpRequest"/> and sets the <see cref="IServiceProvider"/> to a scoped
    /// <see cref="IServiceProvider"/> that knows about the tenant services for the tenant supplied by the Tenant-Id header on the <see cref="HttpRequest"/>.
    /// </summary>
    public partial class TenantScopedServiceProviderMiddleware
    {
        const string TenantIdHeader = "Tenant-ID";
        readonly RequestDelegate _next;
        readonly IDolittleClient _client;
        readonly IHostEnvironment _env;
        readonly ILogger<TenantScopedServiceProviderMiddleware> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantScopedServiceProviderMiddleware"/> class.
        /// </summary>
        /// <param name="next">The <see cref="RequestDelegate"/>.</param>
        /// <param name="client">The <see cref="IDolittleClient"/>.</param>
        /// <param name="env">The <see cref="IHostEnvironment"/>.</param>
        /// <param name="logger">The <see cref="ILogger"/>.</param>
        public TenantScopedServiceProviderMiddleware(RequestDelegate next, IDolittleClient client, IHostEnvironment env, ILogger<TenantScopedServiceProviderMiddleware> logger)
        {
            _next = next;
            _client = client;
            _env = env;
            _logger = logger;
        }

        /// <summary>
        /// Invokes the middleware.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            TenantId tenantId;
            if (Guid.TryParse(context.Request.Headers[TenantIdHeader], out var tenantGuid))
            {
                tenantId = tenantGuid;
            }
            else
            {
                if (!_env.IsDevelopment())
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("No tenant id provided").ConfigureAwait(false);
                    return;
                }

                var availableTenantIds = _client.Tenants.Select(_ => _.Id).ToArray();
                if (availableTenantIds.Length == 0)
                {
                    NoTenantsConfigured();
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsync("No tenants configured").ConfigureAwait(false);
                    return;
                }

                if (availableTenantIds.Contains(TenantId.Development))
                {
                    DefaultingToDevelopmentTenant();
                    tenantId = TenantId.Development;
                }
                else
                {
                    tenantId = availableTenantIds.Select(_ => _.ToString()).OrderBy(_ => _).First();
                    DefaultingToFirstTenant(tenantId);
                }
            }

            using var scope = _client.Services.ForTenant(tenantId).CreateScope();
            context.RequestServices = scope.ServiceProvider;
            await _next(context).ConfigureAwait(false);
        }

        [LoggerMessage(0, LogLevel.Debug, "No tenant configured in the Tenant-ID header. Falling back to the development tenant")]
        partial void DefaultingToDevelopmentTenant();

        [LoggerMessage(0, LogLevel.Debug, "No tenant configured in the Tenant-ID header. Falling back to the first tenant in the tenant list ordered: {Tenant}")]
        partial void DefaultingToFirstTenant(TenantId tenant);

        [LoggerMessage(0, LogLevel.Error, "No tenant configured in the Tenant-ID header and there are no tenants configured for the Dolittle Client")]
        partial void NoTenantsConfigured();

    }
}
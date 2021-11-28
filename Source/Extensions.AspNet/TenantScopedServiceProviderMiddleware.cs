// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.SDK.Tenancy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.SDK.Extensions.AspNet
{
    /// <summary>
    /// Represents a middleware that intercepts <see cref="HttpRequest"/> and sets the <see cref="IServiceProvider"/> to a scoped
    /// <see cref="IServiceProvider"/> that knows about the tenant services for the tenant supplied by the Tenant-Id header on the <see cref="HttpRequest"/>.
    /// </summary>
    public class TenantScopedServiceProviderMiddleware
    {
        readonly RequestDelegate _next;
        readonly IDolittleClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantScopedServiceProviderMiddleware"/> class.
        /// </summary>
        /// <param name="next">The <see cref="RequestDelegate"/>.</param>
        /// <param name="client">The <see cref="IDolittleClient"/>.</param>
        public TenantScopedServiceProviderMiddleware(RequestDelegate next, IDolittleClient client)
        {
            _next = next;
            _client = client;
        }

        /// <summary>
        /// Invokes the middleware.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            using var scope = GetTenantSpecificServiceProvider(context)
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope();
            context.RequestServices = scope.ServiceProvider;
            await _next(context).ConfigureAwait(false);
        }

        IServiceProvider GetTenantSpecificServiceProvider(HttpContext ctx)
        {
            var tenantId = Guid.TryParse(ctx.Request.Headers["Tenant-Id"], out var tenantGuid)
                ? new TenantId { Value = tenantGuid }
                : TenantId.Development;
            return _client.Services.ForTenant(tenantId);
        }
    }
}
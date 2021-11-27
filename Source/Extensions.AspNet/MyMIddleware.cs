// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Threading.Tasks;
using Dolittle.SDK.Tenancy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.SDK.Extensions.AspNet
{
    /// <summary>
    /// 
    /// </summary>
    public class MyMIddleware
    {
        readonly RequestDelegate _next;
        readonly IDolittleClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="MyMIddleware"/> class.
        /// </summary>
        /// <param name="next"></param>
        /// <param name="client"></param>
        public MyMIddleware(RequestDelegate next, IDolittleClient client)
        {
            _next = next;
            _client = client;
        }

        /// <inheritdoc />
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
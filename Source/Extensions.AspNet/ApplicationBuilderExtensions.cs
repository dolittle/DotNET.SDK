// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Builder;

namespace Dolittle.SDK.Extensions.AspNet
{
    /// <summary>
    /// Dolittle Extension methods for <see cref="IApplicationBuilder"/>.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Sets up a middleware that sets up request to be correctly scoped to a specific tenant supplied by the Tenant-Id header on the request.
        /// </summary>
        /// <remarks>Very important that .UseDolittle() is called before setting endpoints.</remarks>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The builder for continuation.</returns>
        public static IApplicationBuilder UseDolittle(this IApplicationBuilder app)
            => app.UseMiddleware<TenantScopedServiceProviderMiddleware>();
    }
}
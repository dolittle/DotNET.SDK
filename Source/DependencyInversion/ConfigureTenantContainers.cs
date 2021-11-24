// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.SDK.DependencyInversion
{
    /// <summary>
    /// The callback for configuring the Tenant-specific IoC containers.
    /// </summary>
    /// <param name="tenant">The <see cref="TenantId"/>.</param>
    /// <param name="serviceCollection">The <see cref="IServiceCollection"/> to configure.</param>
    public delegate void ConfigureTenantServices(TenantId tenant, IServiceCollection serviceCollection);
}
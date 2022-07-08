// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.SDK.DependencyInversion;

/// <summary>
/// Defines a system that can create a Tenant-specific IoC containers from a root service provider.
/// </summary>
public interface ICreateTenantContainers
{
    /// <summary>
    /// Creates an IoC container for a specific tenant using a root provider and a set of tenant specific services.
    /// </summary>
    /// <param name="provider">The root <see cref="IServiceProvider"/> configured in the Client.</param>
    /// <param name="tenant">The <see cref="TenantId"/> to create a container for.</param>
    /// <param name="tenantServices">The <see cref="IServiceCollection"/> of services to configure for the tenant.</param>
    /// <returns>An <see cref="IServiceProvider"/> to use for the specified tenant.</returns>
    IServiceProvider Create(IServiceProvider provider, TenantId tenant, IServiceCollection tenantServices);
}

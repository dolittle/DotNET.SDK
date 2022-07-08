// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.SDK.DependencyInversion;

/// <summary>
/// The callback for creating a Tenant-specific IoC container using a root container, and Tenant-specific services.
/// </summary>
/// <param name="provider">The <see cref="IServiceProvider"/> configured for the Client.</param>
/// <param name="tenant">The <see cref="TenantId"/> to create the provider for.</param>
/// <param name="tenantServices">The <see cref="IServiceCollection"/> of Tenant-specific services to build the provider from.</param>
/// <returns>An <see cref="IServiceProvider"/> to use for the specified Tenant.</returns>
public delegate IServiceProvider CreateTenantServiceProvider(IServiceProvider provider, TenantId tenant, IServiceCollection tenantServices);

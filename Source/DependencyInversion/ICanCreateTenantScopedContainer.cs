// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.SDK.DependencyInversion;

/// <summary>
/// Defines a system that can create a tenant-scoped container from a root provider.
/// </summary>
public interface ICanCreateTenantScopedContainer
{
    /// <summary>
    /// Checks whether a tenant scoped child container  can be created from the given root <see cref="IServiceProvider"/>.
    /// </summary>
    /// <param name="rootProvider">The root <see cref="IServiceProvider"/>.</param>
    /// <returns>True if can be created, false if not.</returns>
    bool CanCreateFrom(IServiceProvider rootProvider);

    /// <summary>
    /// Creates a tenant scoped child container from the root <see cref="IServiceProvider"/> and tenant specific <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="rootProvider">The root <see cref="IServiceProvider"/>.</param>
    /// <param name="tenantScopedServices">The tenant specific additional <see cref="IServiceCollection"/> for the created child container.</param>
    /// <returns>The created tenant scoped child <see cref="IServiceProvider"/> container.</returns>
    IServiceProvider Create(IServiceProvider rootProvider, IServiceCollection tenantScopedServices);
}

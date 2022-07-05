// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.SDK.DependencyInversion;

/// <summary>
/// Defines a system that can create a tenant-scoped container from a root provider.
/// </summary>
public interface ICreateTenantContainers<in TContainer>
    where TContainer : class, IServiceProvider
{
    /// <summary>
    /// Throws an exception if the root <see cref="IServiceProvider"/> container is not of the expected <typeparamref name="TContainer"/> <see cref="Type"/>.
    /// </summary>
    /// <param name="rootContainer">The root <see cref="IServiceProvider"/> container.</param>
    /// <returns>The root container as <see cref="TContainer"/>.</returns>
    public static TContainer RootContainerGuard(IServiceProvider rootContainer)
    {
        if (rootContainer is not TContainer container)
        {
            throw new CannotCreateTenantContainersFromRootContainer<TContainer>(rootContainer);
        }
        return container;
    }
    /// <summary>
    /// Creates a tenant scoped child container from the root <see cref="IServiceProvider"/> and tenant specific <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="rootContainer">The root <see cref="IServiceProvider"/>.</param>
    /// <param name="tenantScopedServices">The tenant specific additional <see cref="IServiceCollection"/> services for the created child container.</param>
    /// <returns>The created tenant scoped child <see cref="IServiceProvider"/> container.</returns>
    IServiceProvider Create(TContainer rootContainer, IServiceCollection tenantScopedServices);
}

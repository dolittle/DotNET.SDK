// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Dolittle.SDK.DependencyInversion;

/// <summary>
/// Extensions on <see cref="IHostBuilder"/> for setting up Dolittle tenant scoped child containers using Lamar. 
/// </summary>
public static class HostExtensions
{
    /// <summary>
    /// Sets up a custom <see cref="ICanCreateTenantScopedContainer"/> to be used in the <see cref="IHost"/> and in the <see cref="TenantScopedProvidersBuilder"/>.
    /// </summary>
    /// <param name="hostBuilder">The <see cref="IHostBuilder"/>.</param>
    /// <param name="instance">The instance of <typeparamref name="TCreator"/>.</param>
    /// <returns></returns>
    public static IHostBuilder UseDolittleTenantContainerCreator<TCreator>(this IHostBuilder hostBuilder, ICanCreateTenantScopedContainer instance = null)
        where TCreator : class, ICanCreateTenantScopedContainer
        => instance is null 
            ? hostBuilder.ConfigureServices((_, services) => services.TryAddEnumerable(ServiceDescriptor.Singleton<ICanCreateTenantScopedContainer, TCreator>()))
            : hostBuilder.ConfigureServices((_, services) => services.TryAddEnumerable(ServiceDescriptor.Singleton(instance)));
    
    /// <summary>
    /// Sets up a custom <see cref="ICanCreateTenantScopedContainer"/> to be used in the <see cref="IHost"/> and in the <see cref="TenantScopedProvidersBuilder"/>.
    /// </summary>
    /// <param name="hostBuilder">The <see cref="IHostBuilder"/>.</param>
    /// <param name="factory">The <see cref="Func{TResult}"/> factory for creating <typeparamref name="TCreator"/> using the service provider.</param>
    /// <returns></returns>
    public static IHostBuilder UseDolittleTenantContainerCreator<TCreator>(this IHostBuilder hostBuilder, Func<IServiceProvider, TCreator> factory)
        where TCreator : class, ICanCreateTenantScopedContainer
        => hostBuilder.ConfigureServices((_, services) => services.TryAddEnumerable(new ServiceDescriptor(typeof(ICanCreateTenantScopedContainer), factory, ServiceLifetime.Singleton)));
}

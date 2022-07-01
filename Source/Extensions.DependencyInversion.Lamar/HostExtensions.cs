// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.DependencyInversion;
using Lamar;
using Lamar.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Dolittle.SDK.Extensions.DependencyInversion.Lamar;

/// <summary>
/// Extensions on <see cref="IHostBuilder"/> for setting up Dolittle tenant scoped child containers using Lamar. 
/// </summary>
public static class HostExtensions
{
    /// <summary>
    /// Sets up Lamar to be used in the <see cref="IHost"/> and in the <see cref="TenantScopedProvidersBuilder"/>.
    /// </summary>
    /// <param name="hostBuilder">The <see cref="IHostBuilder"/>.</param>
    /// <param name="configure">The callback for configuring the Lamar root <see cref="Container"/>.</param>
    /// <returns></returns>
    public static IHostBuilder UseDolittleLamarTenantContainers(this IHostBuilder hostBuilder, Action<HostBuilderContext, ServiceRegistry> configure = null)
        => hostBuilder
            .UseLamar(configure)
            .ConfigureServices(_ => _.TryAddEnumerable(ServiceDescriptor.Singleton<ICanCreateTenantScopedContainer, TenantScopedContainerCreator>()));    
    
    
#if NET5_0_OR_GREATER
    /// <summary> 
    /// Sets up Lamar to be used in the <see cref="IHost"/> and in the <see cref="TenantScopedProvidersBuilder"/>.
    /// </summary>
    /// <param name="hostBuilder">The <see cref="IHostBuilder"/>.</param>
    /// <param name="configure">The callback for configuring the Lamar root <see cref="Container"/>.</param>
    /// <returns></returns>
    public static IHostBuilder UseDolittleLamarTenantContainers(this IHostBuilder hostBuilder, Action<ServiceRegistry> configure)
        => hostBuilder
            .UseLamar(configure)
            .ConfigureServices(_ => _.TryAddEnumerable(ServiceDescriptor.Singleton<ICanCreateTenantScopedContainer, TenantScopedContainerCreator>()));
#endif
#if NETCOREAPP3_1
    /// <summary> 
    /// Sets up Lamar to be used in the <see cref="IHost"/> and in the <see cref="TenantScopedProvidersBuilder"/>.
    /// </summary>
    /// <param name="hostBuilder">The <see cref="IHostBuilder"/>.</param>
    /// <param name="registry">The Lamar root <see cref="ServiceRegistry"/>.</param>
    /// <returns></returns>
    public static IHostBuilder UseDolittleLamarTenantContainers(this IHostBuilder hostBuilder, ServiceRegistry registry)
        => hostBuilder
            .UseLamar(registry)
            .ConfigureServices(_ => _.TryAddEnumerable(ServiceDescriptor.Singleton<ICanCreateTenantScopedContainer, TenantScopedContainerCreator>()));
#endif
}

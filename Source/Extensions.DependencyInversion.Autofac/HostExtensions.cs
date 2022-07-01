// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Autofac;
using Autofac.Builder;
using Autofac.Extensions.DependencyInjection;
using Dolittle.SDK.DependencyInversion;
using Microsoft.Extensions.Hosting;

namespace Dolittle.SDK.Extensions.DependencyInversion.Autofac;

/// <summary>
/// Extensions on <see cref="IHostBuilder"/> for setting up Dolittle tenant scoped child containers using Autofac. 
/// </summary>
public static class HostExtensions
{
    /// <summary>
    /// Sets up the <see cref="AutofacServiceProviderFactory"/> to be used in the <see cref="TenantScopedProvidersBuilder"/>.
    /// </summary>
    /// <param name="hostBuilder">The <see cref="IHostBuilder"/>.</param>
    /// <param name="configureContainer">The callback for configuring the Autofac root <see cref="ContainerBuilder"/>.</param>
    /// <returns></returns>
    public static IHostBuilder UseDolittleAutofacTenantContainers(this IHostBuilder hostBuilder, Action<ContainerBuilder> configureContainer = null)
        => hostBuilder
            .UseServiceProviderFactory(new AutofacServiceProviderFactory(configureContainer))
            .UseDolittleTenantContainerCreator<TenantScopedContainerCreator>();

    /// <summary>
    /// Sets up the <see cref="AutofacServiceProviderFactory"/> as the service provider factory and 
    /// </summary>
    /// <param name="hostBuilder">The <see cref="IHostBuilder"/>.</param>
    /// <param name="containerBuildOptions">The <see cref="ContainerBuildOptions"/>.</param>
    /// <param name="configureContainer">The callback for configuring the Autofac root <see cref="ContainerBuilder"/>.</param>
    /// <returns></returns>
    public static IHostBuilder UseDolittleAutofacTenantContainers(this IHostBuilder hostBuilder, ContainerBuildOptions containerBuildOptions, Action<ContainerBuilder> configureContainer = null)
        => hostBuilder
            .UseServiceProviderFactory(new AutofacServiceProviderFactory(containerBuildOptions, configureContainer))
            .UseDolittleTenantContainerCreator<TenantScopedContainerCreator>();
    
}

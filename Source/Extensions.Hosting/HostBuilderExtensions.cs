// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.DependencyInversion;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dolittle.SDK.Extensions.Hosting
{
    /// <summary>
    /// Static extension methods on the <see cref="IHostBuilder"/> for setting up Dolittle.
    /// </summary>
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// Uses Dolittle by setting it up with the <see cref="IHostBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/>.</param>
        /// <param name="configureSetup">The <see cref="SetupDolittleClient"/> callback for configuring the <see cref="DolittleClientBuilder"/>.</param>
        /// <param name="configureClientConfiguration">The <see cref="Action"/> callback for configuring the <see cref="DolittleClientConfigurationBuilder"/>.</param>
        /// <param name="configureTenantContainers">The <see cref="ConfigureTenantContainers"/> callback.</param>
        /// <returns>The builder for continuation.</returns>
        public static IHostBuilder UseDolittle(
            this IHostBuilder builder,
            SetupDolittleClient configureSetup = default,
            Action<DolittleClientConfigurationBuilder> configureClientConfiguration = default,
            ConfigureTenantContainers configureTenantContainers = default)
        {
            var dolittleSetup = DolittleClient.Setup();
            builder.ConfigureServices(services =>
            {
                configureSetup?.Invoke(dolittleSetup);
                services.AddSingleton<IDolittleClient>(dolittleSetup.Build());
                services.AddSingleton(provider => new DolittleClientService(
                    provider,
                    provider.GetRequiredService<IDolittleClient>(),
                    provider.GetRequiredService<IHostEnvironment>(),
                    configureClientConfiguration,
                    configureTenantContainers));
            });

            return builder;
        }

        /// <summary>
        /// Uses Dolittle by setting it up with the <see cref="IHostBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/>.</param>
        /// <param name="configureSetup">The <see cref="SetupDolittleClient"/> callback for configuring the <see cref="DolittleClientBuilder"/>.</param>
        /// <param name="configureClientConfiguration">The <see cref="ConfigureDolittleClient"/> callback for configuring the <see cref="DolittleClientConfigurationBuilder"/>.</param>
        /// <param name="configureTenantServices">The <see cref="ConfigureTenantServices"/> callback.</param>
        /// <returns>The builder for continuation.</returns>
        public static IHostBuilder UseDolittle(
            this IHostBuilder builder,
            SetupDolittleClient configureSetup = default,
            ConfigureDolittleClient configureClientConfiguration = default,
            ConfigureTenantServices configureTenantServices = default)
        {
            builder.ConfigureServices(services =>
            {
                var dolittleSetup = DolittleClient.Setup();
                configureSetup?.Invoke(dolittleSetup);
                services.AddSingleton<IDolittleClient>(dolittleSetup.Build());
                services.AddSingleton(provider => new DolittleClientService(
                    provider,
                    provider.GetRequiredService<IDolittleClient>(),
                    provider.GetRequiredService<IHostEnvironment>(),
                    configureClientConfiguration,
                    configureTenantServices));
            });

            return builder;
        }
        /// <summary>
        /// Uses Dolittle by setting it up with the <see cref="IHostBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/>.</param>
        /// <param name="configureSetup">The <see cref="Action"/> callback for configuring the <see cref="DolittleClientBuilder"/>.</param>
        /// <param name="configureClientConfiguration">The <see cref="Action"/> callback for configuring the <see cref="DolittleClientConfigurationBuilder"/>.</param>
        /// <param name="configureTenantServices">The <see cref="ConfigureTenantServices"/> callback.</param>
        /// <returns>The builder for continuation.</returns>
        public static IHostBuilder UseDolittle(
            this IHostBuilder builder,
            Action<DolittleClientBuilder> configureSetup,
            Action<DolittleClientConfigurationBuilder> configureClientConfiguration = default,
            ConfigureTenantServices configureTenantServices = default)
        {
            builder.ConfigureServices(services =>
            {
                var dolittleSetup = DolittleClient.Setup();
                configureSetup?.Invoke(dolittleSetup);
                services.AddSingleton<IDolittleClient>(dolittleSetup.Build());
                services.AddSingleton(provider => new DolittleClientService(
                    provider,
                    provider.GetRequiredService<IDolittleClient>(),
                    provider.GetRequiredService<IHostEnvironment>(),
                    configureClientConfiguration,
                    configureTenantServices));
            });

            return builder;
        }
    }
}
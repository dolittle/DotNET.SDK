// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/> for adding setting up the <see cref="IDolittleClient"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="IDolittleClient"/> and <see cref="DolittleClientConfiguration"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="setupClient">The <see cref="SetupDolittleClient"/> callback.</param>
        /// <param name="configureClient">The <see cref="ConfigureDolittleClient"/> callback.</param>
        /// <returns>The <see cref="IServiceCollection"/> for continuation.</returns>
        public static IServiceCollection AddDolittle(this IServiceCollection services, SetupDolittleClient setupClient, ConfigureDolittleClient configureClient)
        {
            var dolittleClient = DolittleClient.Setup(setupClient);
            return services
                .AddSingleton(dolittleClient)
                .AddTransient(provider =>
                {
                    var dolittleConfig = CreateInitialDolittleConfig(provider);
                    configureClient(dolittleConfig);
                    return dolittleConfig;
                })
                .AddSingleton(provider => new DolittleClientService(provider.GetRequiredService<IDolittleClient>(), provider.GetRequiredService<DolittleClientConfiguration>()));
        }

        static DolittleClientConfiguration CreateInitialDolittleConfig(IServiceProvider provider)
        {
            var dolittleConfig = provider.GetService<IConfiguration>()?.Get<DolittleClientConfiguration>() ?? new DolittleClientConfiguration();
            var loggerFactory = provider.GetService<ILoggerFactory>();
            if (loggerFactory != default)
            {
                dolittleConfig.WithLogging(loggerFactory);
            }

            return dolittleConfig
                .WithLogging(loggerFactory)
                .WithServiceProvider(provider);
        }
    }
}
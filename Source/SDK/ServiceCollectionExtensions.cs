// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/> for adding setting up the <see cref="IDolittleClient"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the <see cref="IDolittleClient"/> and <see cref="DolittleClientConfiguration"/> to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="setupClient">The optional <see cref="SetupDolittleClient"/> callback.</param>
    /// <param name="configureClient">The optional <see cref="ConfigureDolittleClient"/> callback.</param>
    /// <returns>The <see cref="IServiceCollection"/> for continuation.</returns>
    public static IServiceCollection AddDolittle(this IServiceCollection services, SetupDolittleClient setupClient = default, ConfigureDolittleClient configureClient = default)
    {
        return services
            .AddDolittleOptions(configureClient)
            .AddDolittleClient(setupClient);
    }

    static IServiceCollection AddDolittleOptions(this IServiceCollection services, ConfigureDolittleClient configureClient = default)
    {
        services
            .AddOptions<DolittleClientConfiguration>()
            .BindConfiguration(nameof(DolittleClientConfiguration))
            .Configure<IServiceProvider>(ConfigureWithDefaultsFromServiceProvider)
            .PostConfigure(clientConfig => configureClient?.Invoke(clientConfig));
        return services;
    }

    static IServiceCollection AddDolittleClient(this IServiceCollection services, SetupDolittleClient setupClient = default)
    {
        var dolittleClient = DolittleClient.Setup(setupClient);
        return services
            .AddSingleton(dolittleClient)
            .AddHostedService<DolittleClientService>();
    }

    static void ConfigureWithDefaultsFromServiceProvider(DolittleClientConfiguration config, IServiceProvider provider)
    {
        config ??= new DolittleClientConfiguration();
        var loggerFactory = provider.GetService<ILoggerFactory>();
        if (loggerFactory != default)
        {
            config.WithLogging(loggerFactory);
        }

        config.WithServiceProvider(provider);
    }
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.SDK.Aggregates.Actors;
using Dolittle.SDK.Builders;
using Dolittle.SDK.Projections.Actors;
using Dolittle.SDK.Proto;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Proto.Cluster;
using Proto.OpenTelemetry;

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
    public static IServiceCollection AddDolittle(this IServiceCollection services, SetupDolittleClient? setupClient = default,
        ConfigureDolittleClient? configureClient = default)
    {
        return services
            .AddDolittleOptions()
            .AddDolittleClient(setupClient, configureClient)
            .AddProtoInfra(GetClusterKinds);
    }

    static IEnumerable<ClusterKind> GetClusterKinds(IServiceProvider serviceProvider)
    {
        var client = (DolittleClient)serviceProvider.GetRequiredService<IDolittleClient>();
        var aggregateTypes = client
            .AggregateRootTypes
            .Types;


        var aggregateKinds = aggregateTypes.Select(aggregateType => AggregateClusterKindFactory.CreateKind(serviceProvider, aggregateType))
            .Select(kind => kind.WithProps(props => props.WithTracing()));

        var projectionTypes = client.ProjectionTypes.Values;

        var projectionKinds = projectionTypes.Select(projectionType => ProjectionClusterKindFactory.CreateKind(serviceProvider, projectionType));

        return aggregateKinds.Concat(projectionKinds);
    }

    static IServiceCollection AddDolittleOptions(this IServiceCollection services)
    {
        services
            .AddOptions<Configurations.Dolittle>()
            .BindConfiguration(nameof(Configurations.Dolittle));
        return services;
    }


    static IServiceCollection AddDolittleClient(this IServiceCollection services, SetupDolittleClient? setupClient = default,
        ConfigureDolittleClient? configureClient = default)
    {
        var dolittleClient = DolittleClient.Setup(setupClient);

        return services
            .AddSingleton(provider => ConfigureWithDefaultsFromServiceProvider(provider, configureClient))
            .AddSingleton(dolittleClient)
            .AddSingleton(dolittleClient.EventTypes)
            .AddHostedService(provider => new DolittleClientService(
                dolittleClient,
                provider.GetRequiredService<DolittleClientConfiguration>(),
                provider.GetRequiredService<ILogger<DolittleClientService>>()));
    }

    static DolittleClientConfiguration ConfigureWithDefaultsFromServiceProvider(IServiceProvider provider, ConfigureDolittleClient? configureClient = default)
    {
        var config = provider.GetService<IOptions<Configurations.Dolittle>>()?.Value;

        var clientConfig = config is not null
            ? DolittleClientConfiguration.FromConfiguration(config)
            : new DolittleClientConfiguration();

        configureClient?.Invoke(clientConfig);

        if (clientConfig.ServiceProvider is null)
        {
            clientConfig.WithServiceProvider(provider);
        }

        return clientConfig;
    }
}

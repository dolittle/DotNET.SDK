// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Dolittle.SDK.Aggregates.Actors;
using Dolittle.SDK.Builders;
using Dolittle.SDK.Proto;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Proto;
using Proto.Cluster;
using Proto.Cluster.Partition;
using Proto.Cluster.Seed;
using Proto.Cluster.SingleNode;
using Proto.DependencyInjection;
using Proto.OpenTelemetry;
using Proto.Remote.GrpcNet;

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
    public static IServiceCollection AddDolittle(this IServiceCollection services, SetupDolittleClient setupClient = default,
        ConfigureDolittleClient configureClient = default)
    {
        return services
            .AddDolittleOptions()
            .AddDolittleClient(setupClient, configureClient)
            .AddProtoInfra(GetClusterKinds);
    }

    static IEnumerable<ClusterKind> GetClusterKinds(IServiceProvider serviceProvider)
    {
        var client = serviceProvider.GetRequiredService<IDolittleClient>() as DolittleClient;
        var aggregateTypes = client
            .AggregateRootTypes
            .Types;

        return aggregateTypes.Select(aggregateType => AggregateClusterKindFactory.CreateKind(serviceProvider, aggregateType))
            .Select(kind => kind.WithProps(props => props.WithTracing()));
    }
    
    static IServiceCollection AddDolittleOptions(this IServiceCollection services)
    {
        services
            .AddOptions<Configurations.Dolittle>()
            .BindConfiguration(nameof(Configurations.Dolittle));
        return services;
    }

    

    static IServiceCollection AddDolittleClient(this IServiceCollection services, SetupDolittleClient setupClient = default,
        ConfigureDolittleClient configureClient = default)
    {
        var dolittleClient = DolittleClient.Setup(setupClient);
        return services
            .AddSingleton(dolittleClient)
            .AddSingleton(dolittleClient.EventTypes)
            .AddHostedService(provider =>
                new DolittleClientService(
                    dolittleClient,
                    ConfigureWithDefaultsFromServiceProvider(provider, configureClient),
                    provider.GetRequiredService<ILogger<DolittleClientService>>()));
    }

    static DolittleClientConfiguration ConfigureWithDefaultsFromServiceProvider(IServiceProvider provider, ConfigureDolittleClient configureClient = default)
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

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Dolittle.SDK.Aggregates.Actors;
using Dolittle.SDK.Builders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Proto;
using Proto.Cluster;
using Proto.Cluster.SingleNode;
using Proto.DependencyInjection;
using Proto.OpenTelemetry;
using Proto.Remote.GrpcNet;

namespace Dolittle.SDK.Proto;

delegate IEnumerable<ClusterKind> GetClusterKinds(IServiceProvider provider);

static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProtoInfra(this IServiceCollection services, GetClusterKinds getClusterKinds)
    {
        return AddProtoCluster(services, getClusterKinds)
            .AddSingleton<GetServiceProviderForTenant>(sp =>
            {
                var client = sp.GetRequiredService<IDolittleClient>();
                return async tenantId =>
                {
                    await client.Connected;
                    return client.Services.ForTenant(tenantId);
                };
            })
            .AddSingleton<AggregateUnloadTimeout>(sp =>
            {
                var dolittleClientConfiguration = sp.GetRequiredService<DolittleClientConfiguration>();
                var timeout = dolittleClientConfiguration.AggregateIdleTimeout;

                return () => timeout;
            });
    }

    public static IServiceCollection AddProtoCluster(this IServiceCollection self, GetClusterKinds getClusterKinds)
    {
        self.AddSingleton(p =>
        {
            var loggerFactory = p.GetRequiredService<ILoggerFactory>();
            global::Proto.Log.SetLoggerFactory(new ProtoInternalsLoggerFactoryProxy(loggerFactory));


            var r = GrpcNetRemoteConfig.BindToLocalhost();

            var c = ClusterConfig.Setup("sdk", new SingleNodeProvider(), new SingleNodeLookup()) with
            {
                ClusterKinds = getClusterKinds(p).ToImmutableList()
            };

            var system = new ActorSystem()
                .WithRemote(r)
                .WithCluster(c)
                .WithServiceProvider(p);

            return system;
        });

        self.AddSingleton(p => p.GetRequiredService<ActorSystem>().Cluster());
        self.AddSingleton(p => p.GetRequiredService<ActorSystem>().Root.WithTracing());
        self.AddHostedService(p =>
            new ProtoActorLifecycleHost(
                p.GetRequiredService<ActorSystem>(),
                p.GetRequiredService<ILogger<ProtoActorLifecycleHost>>(),
                p.GetRequiredService<IHostApplicationLifetime>(),
                false));

        return self;
    }
}

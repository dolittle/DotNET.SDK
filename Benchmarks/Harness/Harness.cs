// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Loggers;
using Docker.DotNet;
using Dolittle.SDK.Builders;
using Dolittle.SDK.Microservices;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.DependencyInjection;
using Environment = Dolittle.SDK.Microservices.Environment;

namespace Dolittle.SDK.Benchmarks.Harness;

/// <summary>
/// Represents a system that can setup a benchmarking harness using Docker to host the Runtime and a reusable MongoDB.
/// </summary>
public class Harness : IAsyncDisposable
{
    static readonly MicroserviceId _microserviceId = "1567f58f-c58e-4752-bf3d-b8a89deb78d0";
    static readonly Environment _environment = "Benchmarking";
    
    readonly ILogger _logger;
    readonly MongoDB _mongoContainer;
    readonly Runtime _runtimeContainer;
    readonly IDockerClient _dockerClient;
    readonly IList<string> _createdDatabases;

    Harness(ILogger logger, MongoDB mongoContainer, Runtime runtimeContainer, IDolittleClient dolittleClient, IDockerClient dockerClient, IList<string> createdDatabases)
    {
        _logger = logger;
        _mongoContainer = mongoContainer;
        _runtimeContainer = runtimeContainer;
        _dockerClient = dockerClient;
        _createdDatabases = createdDatabases;
        Client = dolittleClient;
    }
    
    /// <summary>
    /// Gets the <see cref="IDolittleClient"/> to use in the benchmark.
    /// </summary>
    public IDolittleClient Client { get; }

    /// <summary>
    /// Configures the benchmarking harness for the provided tenants and returns the connected Dolittle client.
    /// </summary>
    /// <param name="setup">The setup callback to use to setup the Dolittle client.</param>
    /// <param name="configure">The configure callback to use to configure the Dolittle client.</param>
    /// <param name="tenants">The tenants to setup the benchmarking harness for.</param>
    /// <returns>A <see cref="Task{TResult}"/> that, when resolved, returns a connected <see cref="IDolittleClient"/>.</returns>
    public static async Task<Harness> Start(ILogger logger, SetupDolittleClient setup, ConfigureDolittleClient configure, params TenantId[] tenants)
    {
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(30));

        try
        {
            var dockerClient = new DockerClientConfiguration().CreateClient();

            var mongo = await MongoDB.StartOrReuseExistingContainer("dolittle-dotnet-sdk-benchmark-mongo", dockerClient, logger, cts.Token);

            var runtimeConfiguration = new Dictionary<string, string>();
            var createdDatabases = new List<string>();
            PopulateRuntimeConfiguration(runtimeConfiguration, createdDatabases, tenants, mongo.DockerHost);

            var environmentVariables = runtimeConfiguration.Select(_ => $"{_.Key.Replace(":", "__").ToUpperInvariant()}={_.Value}").ToList();
            var runtime = await Runtime.StartContainer("dolittle-dotnet-sdk-benchmark-runtime", environmentVariables, dockerClient, logger, cts.Token);

            var dolittleClient = await DolittleClient
                .Setup(setup)
                .Connect(_ =>
                {
                    configure(_);
                    _.WithRuntimeOn("localhost", runtime.ExposedPrivatePort);
                    _.WithServiceProvider(new ServiceCollection().BuildServiceProvider());
                }, cts.Token)
                .ConfigureAwait(false);

            return new Harness(logger, mongo, runtime, dolittleClient, dockerClient, createdDatabases);
        }
        catch (TaskCanceledException) when (cts.IsCancellationRequested)
        {
            logger.WriteLineError("Could not start harness within 30 seconds, giving up...");
            throw;
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await Client.Disconnect().ConfigureAwait(false);
        await _runtimeContainer.DisposeAsync().ConfigureAwait(false);
        await _mongoContainer.DropDatabases(_createdDatabases).ConfigureAwait(false);
        _dockerClient.Dispose();
    }

    static void PopulateRuntimeConfiguration(IDictionary<string, string> configuration, IList<string> databases, IEnumerable<TenantId> tenants, string mongoDockerHost)
    {
        configuration["dolittle:runtime:eventstore:backwardscompatibility:version"] = "V7";
        configuration["dolittle:runtime:platform:microserviceID"] = _microserviceId.ToString();
        configuration["dolittle:runtime:platform:environment"] = _environment.ToString();

        foreach (var tenant in tenants)
        {
            var eventStoreName = Guid.NewGuid().ToString();
            configuration[$"dolittle:runtime:tenants:{tenant}:resources:eventStore:servers:0"] = mongoDockerHost;
            configuration[$"dolittle:runtime:tenants:{tenant}:resources:eventStore:database"] = eventStoreName;
            databases.Add(eventStoreName);
            
            var projectionsName = Guid.NewGuid().ToString();
            configuration[$"dolittle:runtime:tenants:{tenant}:resources:projections:servers:0"] = mongoDockerHost;
            configuration[$"dolittle:runtime:tenants:{tenant}:resources:projections:database"] = projectionsName;
            databases.Add(projectionsName);
            
            var embeddingsName = Guid.NewGuid().ToString();
            configuration[$"dolittle:runtime:tenants:{tenant}:resources:embeddings:servers:0"] = mongoDockerHost;
            configuration[$"dolittle:runtime:tenants:{tenant}:resources:embeddings:database"] = embeddingsName;
            databases.Add(embeddingsName);
            
            var readModelsName = Guid.NewGuid().ToString();
            configuration[$"dolittle:runtime:tenants:{tenant}:resources:readModels:host"] = $"mongodb://{mongoDockerHost}:27017";
            configuration[$"dolittle:runtime:tenants:{tenant}:resources:readModels:database"] = readModelsName;
            databases.Add(readModelsName);
        }
    }
}

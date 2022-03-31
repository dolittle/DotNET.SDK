// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Loggers;
using Docker.DotNet;
using Docker.DotNet.Models;
using Newtonsoft.Json;

namespace Dolittle.SDK.Benchmarks.Harness;

public class Runtime : IAsyncDisposable
{
    const int PrivatePort = 50053;
    const int WebPort = 8001;
    const string RuntimeImage = "dolittle/runtime:8.0.0";

    readonly string _containerId;
    readonly IDockerClient _docker;
    
    Runtime(ContainerListResponse status, IDockerClient docker)
    {
        _containerId = status.ID;
        _docker = docker;
    }
    
    public ushort ExposedPrivatePort { get; private set; }

    async Task Initialize(CancellationToken cancellationToken)
    {
        var client = new HttpClient();
        while (!cancellationToken.IsCancellationRequested)
        {
            var status = await _docker.GetContainerById(_containerId, cancellationToken).ConfigureAwait(false);
            if (status.TryGetPublishedPort(WebPort, out var publicWebPort))
            {
                try
                {
                    var response = await client.GetAsync($"http://localhost:{publicWebPort}/healthz", cancellationToken).ConfigureAwait(false);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        ExposedPrivatePort = GetExposedPrivatePort(status);
                        break;
                    }
                }
                catch
                {
                    // Retry when connection fails
                }
            }

            await Task.Delay(100, cancellationToken).ConfigureAwait(false);
        }
    }

    ushort GetExposedPrivatePort(ContainerListResponse status)
    {
        if (status.TryGetPublishedPort(PrivatePort, out var publicPort))
        {
            return publicPort;
        }

        throw new Exception("Private port not exposed on the Runtime");
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await _docker.Containers.StopContainerAsync(_containerId, new ContainerStopParameters {WaitBeforeKillSeconds = 0}).ConfigureAwait(false);
    }

    public static async Task<Runtime> StartContainer(string containerName, IList<string> environmentVariables, IDockerClient docker, ILogger logger, CancellationToken cancellationToken = default)
    {
        logger.WriteLineInfo("Starting a new Runtime container...");
        var status = await docker.StartNewContainer(
            new CreateContainerParameters
            {
                Name = $"{containerName}-{Guid.NewGuid()}",
                Image = RuntimeImage,
                Env = environmentVariables,
                ExposedPorts = new Dictionary<string, EmptyStruct>
                {
                        [$"{PrivatePort}/tcp"] = new(),
                        [$"{WebPort}/tcp"] = new(),
                },
                HostConfig = new HostConfig
                {
                    PortBindings = new Dictionary<string, IList<PortBinding>>
                    {
                        [$"{PrivatePort}/tcp"] = new List<PortBinding>{ new() },
                        [$"{WebPort}/tcp"] = new List<PortBinding>{ new() },
                    }
                },
            },
            cancellationToken).ConfigureAwait(false);
        
        var runtime = new Runtime(status, docker);
        await runtime.Initialize(cancellationToken).ConfigureAwait(false);
        logger.WriteLineInfo("Runtime is ready");
        return runtime;
    }
}

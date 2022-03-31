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

namespace Dolittle.SDK.Benchmarks.Harness;

/// <summary>
/// Represents a Runtime instance running in Docker.
/// </summary>
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
    
    /// <summary>
    /// Gets the port number of the Private-endpoint published to the host.
    /// </summary>
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

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await _docker.Containers.StopContainerAsync(_containerId, new ContainerStopParameters {WaitBeforeKillSeconds = 0}).ConfigureAwait(false);
    }

    /// <summary>
    /// Starts a new Runtime container with the specified name prefix and environmental variables in Docker.
    /// </summary>
    /// <param name="containerNamePrefix">The prefix to use for the generated container name.</param>
    /// <param name="environmentVariables">The environmental variables to use for the Runtime container.</param>
    /// <param name="docker">The <see cref="IDockerClient"/> to use to start the container.</param>
    /// <param name="logger">The logger to use for logging.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to use to cancel the operation.</param>
    /// <returns>A <see cref="Task{TResult}"/> that, when resolved, returns the <see cref="Runtime"/> representing the Runtime container.</returns>
    public static async Task<Runtime> StartContainer(string containerNamePrefix, IList<string> environmentVariables, IDockerClient docker, ILogger logger, CancellationToken cancellationToken = default)
    {
        logger.WriteLineInfo("Starting a new Runtime container...");
        var status = await docker.StartNewContainer(
            new CreateContainerParameters
            {
                Name = $"{containerNamePrefix}-{Guid.NewGuid()}",
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

    static ushort GetExposedPrivatePort(ContainerListResponse status)
    {
        if (status.TryGetPublishedPort(PrivatePort, out var publicPort))
        {
            return publicPort;
        }

        throw new FailedToStartRuntime($"The private port {PrivatePort} is not published");
    }
}

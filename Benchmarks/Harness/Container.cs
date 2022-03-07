// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Loggers;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace Dolittle.Benchmarks.Harness;

public abstract class Container : IContainer
{
    readonly BoundPorts _boundPorts;
    readonly ILogger _logger;
    bool _started;

    protected Container(
        IDockerClient client,
        string image,
        string tag,
        BoundPorts boundPorts,
        ILogger logger)
    {
        Image = image;
        Tag = tag;
        Client = client;
        _boundPorts = boundPorts;
        _logger = logger;
    }

    protected string Image { get; }
    protected string Tag { get; }
    protected IDockerClient Client { get; }
    protected string ContainerId { get; private set; }

    public async Task Start()
    {
        if (_started)
        {
            return;
        }
        _logger.WriteLine(LogKind.Info, $"Starting {this}");
        var exposesPorts = Configuration.CreateExposedPorts(_boundPorts);
        var portBindings = Configuration.CreatePortBindings(_boundPorts);
        await Client.Images.CreateImageAsync(
            new ImagesCreateParameters
            {
                Tag = Tag,
                FromImage = $"{Image}:{Tag}"
            },
            null,
            new Progress<JSONMessage>()).ConfigureAwait(false);
        var createContainerParameters = new CreateContainerParameters
        {
            Image = $"{Image}:{Tag}",
            ExposedPorts = exposesPorts,
            HostConfig = new HostConfig
            {
                PortBindings = portBindings
            },
        };
        ModifyCreateContainerParameters(createContainerParameters);
        ContainerId = (await Client.Containers.CreateContainerAsync(createContainerParameters).ConfigureAwait(false)).ID;
        await Client.Containers.StartContainerAsync(ContainerId, new ContainerStartParameters()).ConfigureAwait(false);
        await WaitUntilContainerStarted().ConfigureAwait(false);
        _started = true;
    }

    public async Task Stop()
    {
        if (!_started)
        {
            return;
        }
        _logger.WriteLine(LogKind.Info, $"Stopping {this}");
        // await Client.Containers.StopContainerAsync(ContainerId, new ContainerStopParameters()).ConfigureAwait(false);
        await Client.Containers.KillContainerAsync(ContainerId, new ContainerKillParameters()).ConfigureAwait(false);
        await Client.Containers.RemoveContainerAsync(ContainerId, new ContainerRemoveParameters{Force = true, RemoveLinks = false, RemoveVolumes = true}).ConfigureAwait(false);
        _started = false;
    }

    public virtual async ValueTask DisposeAsync()
    {
        _logger.WriteLine(LogKind.Info, $"Disposing {this}");
        _boundPorts?.Dispose();
        await Stop().ConfigureAwait(false);
    }
    
    public override string ToString()
        => $"{Image}:{Tag} container";


    protected abstract Task WaitUntilContainerStarted();

    protected virtual void ModifyCreateContainerParameters(CreateContainerParameters parameters)
    {
    }
}

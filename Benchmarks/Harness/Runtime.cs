// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Loggers;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace Dolittle.Benchmarks.Harness;

public class Runtime : IAsyncDisposable
{
    const string Image = "dolittle/runtime";

    readonly DockerClient _client;
    readonly RuntimeConfig _config;
    readonly ILogger _logger;

    string _containerId;
    bool _started;

    public Runtime(DockerClient client, RuntimeConfig config, ILogger logger)
    {
        _client = client;
        _config = config;
        _logger = logger;
        Endpoints = CreateEndpoints();
    }

    public RuntimeEndpoints Endpoints { get; }

    public override string ToString()
        => $"Runtime with tag {_config.Tag} and {Endpoints}";

    public async Task Start()
    {
        if (_started)
        {
            return;
        }
        _logger.WriteLine(LogKind.Info, $"Starting {this}");
        // await _client.Images.CreateImageAsync(
        //     new ImagesCreateParameters
        //     {
        //         FromImage = Image,
        //         Tag = _config.Tag,
        //         
        //     },
        //     null,
        //     new Progress<JSONMessage>()).ConfigureAwait(false);
        
        _containerId = (await _client.Containers.CreateContainerAsync(new CreateContainerParameters
        {
            Image = $"{Image}:{_config.Tag}",
            ExposedPorts = _config.PortBindings.ToDictionary(_ => _.Key, _ => new EmptyStruct()),
            HostConfig = new HostConfig
            {
                PortBindings = _config.PortBindings.ToDictionary(_ => _.Key, _ => _.Value)
            }
        }).ConfigureAwait(false)).ID;
        await _client.Containers.StartContainerAsync(_containerId, new ContainerStartParameters()).ConfigureAwait(false);
        await RuntimeStarted();
        _started = true;
    }
    public async Task Stop()
    {
        if (!_started)
        {
            return;
        }
        _logger.WriteLine(LogKind.Info, $"Stopping {this}");
        await _client.Containers.StopContainerAsync(_containerId, new ContainerStopParameters()).ConfigureAwait(false);
        _started = false;
    }

    public async ValueTask DisposeAsync()
    {
        _logger.WriteLine(LogKind.Info, $"Disposing {this}");
        _config?.Dispose();
        await Stop().ConfigureAwait(false);
    }

    RuntimeEndpoints CreateEndpoints()
    {
        var ports = _config.PortBindings.Select(binding =>
        {
            var containerPort = int.Parse(binding.Key.Split('/')[0]);
            var hostPort = int.Parse(binding.Value.First().HostPort);
            return (containerPort, hostPort);
        }).ToArray();
        return new RuntimeEndpoints(
            ports.Single(_ => _.containerPort == RuntimeConfig.DefaultPrivatePort).hostPort,
            ports.Single(_ => _.containerPort == RuntimeConfig.DefaultPublicPort).hostPort);
    }

    async Task RuntimeStarted()
    {
        // Read logs and stop when Runtime is ready
        // var logs = await _client.Containers.GetContainerLogsAsync(
        //     createdContainer.ID, true, new ContainerLogsParameters
        //     {
        //         Follow = true,
        //         ShowStdout = true,
        //         Timestamps = false
        //     }).ConfigureAwait(false);
        // while (true)
        // {
        //     await using var outMem = new MemoryStream();
        //     Console.WriteLine("Copying");
        //     await logs.CopyOutputToAsync(Stream.Null, outMem, Stream.Null, CancellationToken.None).ConfigureAwait(false);
        //     
        //     
        //     Console.WriteLine("Done");
        //     outMem.Seek(0, SeekOrigin.Begin);
        //     using var outRdr = new StreamReader(outMem);
        //     while (!outRdr.EndOfStream)
        //     {
        //         var line = await outRdr.ReadLineAsync().ConfigureAwait(false);
        //         Console.WriteLine(line);
        //     }
        //     await Task.Delay(500);
        // }
        await Task.Delay(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
    }
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Loggers;
using Docker.DotNet;

namespace Dolittle.Benchmarks.Harness;

public class RuntimeContainer : Container
{
    public const int DefaultPrivatePort = 50053;
    public const int DefaultPublicPort = 50052;

    public RuntimeContainer(
        IDockerClient client,
        string tag,
        BoundPort privatePort,
        BoundPort publicPort,
        ILogger logger)
        : base(
            client,
            "dolittle/runtime",
            tag,
            new BoundPorts((privatePort, DefaultPrivatePort), (publicPort, DefaultPublicPort)),
            logger)
    {
        Endpoints = new RuntimeEndpoints(privatePort.Port, publicPort.Port);
    }
    
    public RuntimeEndpoints Endpoints { get; }

    protected override Task WaitUntilContainerStarted()
        => Task.Delay(TimeSpan.FromSeconds(2));

}

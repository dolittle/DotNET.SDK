// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Loggers;
using Docker.DotNet;

namespace Dolittle.Benchmarks.Harness;

public class RuntimeDevelopmentContainer : Container, IRuntimeWithMongo
{
    public RuntimeDevelopmentContainer(
        IDockerClient client,
        string tag,
        BoundPort privatePort,
        BoundPort publicPort,
        BoundPort mongoPort,
        ILogger logger)
        : base(
            client,
            "dolittle/runtime",
            $"{tag}-development",
            new BoundPorts(
                (privatePort, RuntimeContainer.DefaultPrivatePort),
                (publicPort, RuntimeContainer.DefaultPublicPort),
                (mongoPort, MongoDbContainer.DefaultPort)),
            logger)
    {
        Endpoints = new RuntimeEndpoints(privatePort.Port, publicPort.Port);
    }

    protected override Task WaitUntilContainerStarted()
        => Task.Delay(TimeSpan.FromSeconds(5));

    public RuntimeEndpoints Endpoints { get; }
    
}
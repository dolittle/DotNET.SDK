// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Loggers;
using Docker.DotNet;

namespace Dolittle.Benchmarks.Harness;

public class MongoDbContainer : Container
{
    public const int DefaultPort = 27017;
    
    public MongoDbContainer(
        IDockerClient client,
        string tag,
        BoundPort port,
        ILogger logger)
        : base(
            client,
            "dolittle/mongodb",
            tag,
            new BoundPorts((port, DefaultPort)),
            logger)
    {
    }
    
    protected override Task WaitUntilContainerStarted()
        => Task.Delay(TimeSpan.FromSeconds(2));
}

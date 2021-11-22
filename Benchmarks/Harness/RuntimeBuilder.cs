// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using BenchmarkDotNet.Loggers;
using Docker.DotNet;

namespace Dolittle.Benchmarks.Harness;

public class RuntimeBuilder
{
    readonly DockerClient _dockerClient;
    readonly OpenPortPool _portPool;
    readonly ILogger _logger;

    public RuntimeBuilder(DockerClient dockerClient, OpenPortPool portPool, ILogger logger)
    {
        _dockerClient = dockerClient;
        _portPool = portPool;
        _logger = logger;
    }

    public Runtime Build()
    {
        var (privatePort, publicPort, mongoPort) = (_portPool.Find(), _portPool.Find(), _portPool.Find());
        var tag = "latest-development";
        return new Runtime(_dockerClient, new RuntimeConfig(tag, privatePort, publicPort, mongoPort), _logger);
    }
}
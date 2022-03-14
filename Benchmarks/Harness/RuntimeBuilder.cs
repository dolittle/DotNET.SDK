// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Docker.DotNet;

namespace Dolittle.Benchmarks.Harness;

public class RuntimeBuilder
{
    readonly IDockerClient _dockerClient;
    readonly RuntimeWithMongoFactory _runtimeWithMongoFactory;

    public RuntimeBuilder(IDockerClient dockerClient, RuntimeWithMongoFactory runtimeWithMongoFactory)
    {
        _dockerClient = dockerClient;
        _runtimeWithMongoFactory = runtimeWithMongoFactory;
    }

    public IRuntimeWithMongo Build(string runtimeTag = "latest", string mongoDbTag = "latest")
        => _runtimeWithMongoFactory.Create(_dockerClient, runtimeTag, mongoDbTag);

    public IRuntimeWithMongo BuildDevelopment(string tag = "7.8.1-peregrin.14")
        => _runtimeWithMongoFactory.CreateDevelopment(_dockerClient, tag);
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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

    public IRuntimeWithMongo Build(string runtimeTag = "9.0.0-hadhafang.10", string mongoDbTag = "latest")
        => _runtimeWithMongoFactory.Create(_dockerClient, runtimeTag, mongoDbTag);

    public IRuntimeWithMongo BuildDevelopment(string tag = "9.0.0-hadhafang.10")
        => _runtimeWithMongoFactory.CreateDevelopment(_dockerClient, tag);
}

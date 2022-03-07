// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using BenchmarkDotNet.Loggers;
using Docker.DotNet;

namespace Dolittle.Benchmarks.Harness;

public class Harness : IDisposable
{
    static Harness Instance;

    readonly DockerClient _dockerClient;
    readonly RuntimeWithMongoFactory _runtimeWithMongoFactory;

    Harness(ILogger logger)
    {
        _dockerClient = new DockerClientConfiguration().CreateClient();
        _runtimeWithMongoFactory = new RuntimeWithMongoFactory(new OpenPortPool(), logger);
    }

    public static Harness Setup(ILogger logger)
        => Instance ??= new Harness(logger);

    public RuntimeBuilder SetupRuntime() => new(_dockerClient, _runtimeWithMongoFactory);

    public void Dispose() => _dockerClient?.Dispose();
}

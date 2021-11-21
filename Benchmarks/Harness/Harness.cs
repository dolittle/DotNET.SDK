// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using BenchmarkDotNet.Loggers;
using Docker.DotNet;

namespace Dolittle.Benchmarks.Harness;

public class Harness : IDisposable
{
    readonly ILogger _logger;
    static Harness Instance;

    readonly DockerClient _dockerClient;
    readonly OpenPortPool _portPool = new();

    Harness(ILogger logger)
    {
        _logger = logger;
        _dockerClient = new DockerClientConfiguration().CreateClient();
    }

    public static Harness Setup(ILogger logger)
        => Instance ??= new Harness(logger);

    public RuntimeBuilder SetupRuntime() => new(_dockerClient, _portPool, _logger);

    public void Dispose() => _dockerClient?.Dispose();
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using BenchmarkDotNet.Loggers;
using Docker.DotNet.Models;

namespace Dolittle.Benchmarks.Harness;

public class RuntimeConfig : IDisposable
{
    readonly Dictionary<string, IList<PortBinding>> _ports = new();
    readonly BoundPort _privatePort;
    readonly BoundPort _publicPort;
    readonly BoundPort _mongoPort;
    public const int DefaultPrivatePort = 50053;
    public const int DefaultPublicPort = 50052;
    public const int DefaultMongoPort = 27017;

    public RuntimeConfig(string tag, BoundPort privatePort, BoundPort publicPort, BoundPort mongoPort)
    {
        _privatePort = privatePort;
        _publicPort = publicPort;
        _mongoPort = mongoPort;
        Tag = tag;
        AddPortBinding(DefaultPrivatePort, privatePort.Port);
        AddPortBinding(DefaultPublicPort, publicPort.Port);
        AddPortBinding(DefaultMongoPort, mongoPort.Port);
    }
    public string Tag { get; }

    public IReadOnlyDictionary<string, IList<PortBinding>> PortBindings => _ports;

    public void Dispose()
    {
        _privatePort?.Dispose();
        _publicPort?.Dispose();
        _mongoPort?.Dispose();
    }

    void AddPortBinding(int containerPort, int hostPort)
    {
        _ports.Add($"{containerPort}/tcp", new List<PortBinding>{new() { HostPort = hostPort.ToString(), HostIP = "localhost"}});
    }
    
}
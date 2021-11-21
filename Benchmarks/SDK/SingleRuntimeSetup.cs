// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Loggers;
using Dolittle.SDK;

namespace Dolittle.Benchmarks.SDK;

public class SingleRuntimeSetup
{
    Harness.Harness _harness; 
    Harness.Runtime _singleRuntime;
    
    [GlobalSetup]
    public void GlobalSetup()
    {
        var logger = ConsoleLogger.Default;
        logger.WriteLine(LogKind.Info, "Setting up harness");
        _harness = Harness.Harness.Setup(logger);
        _singleRuntime = _harness.SetupRuntime().Build();
    }
    
    [IterationSetup]
    public virtual void IterationSetup()
    {
        Console.WriteLine($"Starting runtime {_singleRuntime.Endpoints}");
        _singleRuntime.Start().Wait();
    }

    [IterationCleanup]
    public virtual void IterationCleanup()
    {
        Console.WriteLine($"stopping runtime {_singleRuntime.Endpoints}");
        _singleRuntime.Stop().Wait();
    }

    [GlobalCleanup]
    public async Task GlobalCleanup()
    {
        Console.WriteLine("Cleaning up harness");
        _harness.Dispose();
        if (_singleRuntime is not null)
        {
            await _singleRuntime.DisposeAsync().ConfigureAwait(false);
        }
    }
    
    public DolittleClientBuilder GetClientBuilder() => DolittleClient
        .ForMicroservice("7e5981aa-8ffb-44de-8a32-c35581cf1dcd")
        .WithRuntimeOn("localhost", (ushort)_singleRuntime.Endpoints.Private);

}
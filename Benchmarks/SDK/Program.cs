// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using BenchmarkDotNet.Running;
using Dolittle.Benchmarks.SDK.EventStore.with_1_tenant;

namespace Dolittle.Benchmarks.SDK;

// Inspiration from https://github.com/dotnet/performance/blob/main/src/harness/BenchmarkDotNet.Extensions/Extensions.cs

sealed class Program
{
    static int Main(string[] args)
    {
        var argsList = new List<string>(args);
        var config = BenchmarkConfig.Create();
        // return BenchmarkSwitcher
        //     .FromAssembly(typeof(Program).Assembly)
        //     .Run(argsList.ToArray(), config)
        //     .ToExitCode();
        return new[] {BenchmarkRunner.Run<committing_100_events_with_warmup>(config, args)}.ToExitCode();
    }

}

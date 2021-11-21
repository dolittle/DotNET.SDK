// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using Dolittle.Benchmarks.Harness;

namespace Dolittle.Benchmarks.SDK;

// Inspiration from https://github.com/dotnet/performance/blob/main/src/harness/BenchmarkDotNet.Extensions/Extensions.cs

class Program
{
    static int Main(string[] args)
    {
        var argsList = new List<string>(args);
        var config = BenchmarkConfig.Create();
        return BenchmarkSwitcher
            .FromAssembly(typeof(Program).Assembly)
            .Run(argsList.ToArray(), config)
            .ToExitCode();
    }

}
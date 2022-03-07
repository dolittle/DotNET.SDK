// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Loggers;
using Dolittle.SDK;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dolittle.Benchmarks.SDK;

public class SingleRuntimeSetup
{
    Harness.Harness _harness; 
    Harness.IRuntimeWithMongo _singleRuntime;
    
    [GlobalSetup]
    public void GlobalSetup()
    {
        var logger = ConsoleLogger.Default;
        logger.WriteLine(LogKind.Info, "Setting up harness");
        _harness = Harness.Harness.Setup(logger);
        _singleRuntime = _harness.SetupRuntime().BuildDevelopment();
    }
    
    [IterationSetup]
    public virtual void IterationSetup()
    {
        _singleRuntime.Start().Wait();
    }

    [IterationCleanup]
    public virtual void IterationCleanup()
    {
        _singleRuntime.Stop().Wait();
    }

    [GlobalCleanup]
    public async Task GlobalCleanup()
    {
        _harness.Dispose();
        if (_singleRuntime is not null)
        {
            await _singleRuntime.DisposeAsync().ConfigureAwait(false);
        }
    }

    public IDolittleClient GetConnectedClient(bool withDiscovery = false)
    {
        var host = Host.CreateDefaultBuilder().UseDolittle(withDiscovery ? _ => _.WithoutDiscovery() : null).Build();
        return host.GetDolittleClient(_ => _.WithRuntimeOn("localhost", (ushort)_singleRuntime.Endpoints.Private).WithServiceProvider(new ServiceCollection().BuildServiceProvider())).GetAwaiter().GetResult();
    }
    

}

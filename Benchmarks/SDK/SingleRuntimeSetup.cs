// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Loggers;
using Dolittle.Benchmarks.SDK.EventStore.with_1_tenant;
using Dolittle.SDK;
using Dolittle.SDK.Builders;
using Dolittle.SDK.Events.Store;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;

namespace Dolittle.Benchmarks.SDK;

public class SingleRuntimeSetup
{
    Harness.Harness _harness; 
    Harness.IRuntimeWithMongo _singleRuntime;
    protected IEventStore _eventStore;
    
    [GlobalSetup]
    public void GlobalSetup()
    {
        var logger = ConsoleLogger.Default;
        logger.WriteLine(LogKind.Info, "Setting up harness");
        _harness = Harness.Harness.Setup(logger);
        _singleRuntime = _harness.SetupRuntime().BuildDevelopment();

        _singleRuntime.Start().Wait();
        var client = GetConnectedClient();
        _eventStore = client.EventStore.ForTenant(TenantId.Development);
        _eventStore.Commit(_ => _.CreateEvent(new some_event())
            .FromEventSource("source")
            .WithEventType("3065740c-f2e3-4ef4-9738-fe2b1a104399")).Wait();
        
    }
    
    [IterationSetup]
    public virtual void IterationSetup()
    {
        // _singleRuntime.Start().Wait();
    }

    [IterationCleanup]
    public virtual void IterationCleanup()
    {
        // _singleRuntime.Stop().Wait();
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
        => GetConnectedClient(_ => _.WithoutDiscovery());
    public IDolittleClient GetConnectedClient(SetupDolittleClient setup)
        => Host.CreateDefaultBuilder().UseDolittle(setup).Build().GetDolittleClient(_ => _
            .WithRuntimeOn("localhost", (ushort)_singleRuntime.Endpoints.Private)
            .WithServiceProvider(new ServiceCollection().BuildServiceProvider())
            .WithLogging(new NullLoggerFactory())).GetAwaiter().GetResult();
}

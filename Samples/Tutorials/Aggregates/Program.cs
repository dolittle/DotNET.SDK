// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Sample code for the tutorial at https://dolittle.io/tutorials/getting-started/csharp/

using System;
using Dolittle.SDK;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder()
    .UseDolittle(configureClientConfiguration: _ => _.WithTenantServices((_, services) => services.AddSingleton<SomeTenantService>()))
    .ConfigureServices(_ => _.AddSingleton<SomeGlobalService>())
    .Build();

await host.StartAsync();

var client = await host.GetDolittleClient();

await client.Aggregates
    .ForTenant(TenantId.Development)
    .Get<Kitchen>("Dolittle Tacos4")
    .Perform(kitchen => kitchen.PrepareDish("Bean Blaster Taco", "Mr. Taco"));

await host.WaitForShutdownAsync();


public class SomeGlobalService
{
    public void DoStuff() => Console.WriteLine($"Doing stuff");
}
public class SomeTenantService
{
    readonly TenantId _tenant;

    public SomeTenantService(TenantId tenant)
    {
        _tenant = tenant;
    }

    public void DoStuff() => Console.WriteLine($"Doing stuff for tenant {_tenant}");
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Sample code for the tutorial at https://dolittle.io/tutorials/getting-started/csharp/

using Dolittle.SDK;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder()
    .UseDolittle()
    .Build();

await host.StartAsync();

var client = await host.GetDolittleClient();
await client.EventStore
    .ForTenant(TenantId.Development)
    .CommitEvent(new DishPrepared("Bean Blaster Taco", "Mr. Taco"), "Dolittle Tacos");

await host.WaitForShutdownAsync();

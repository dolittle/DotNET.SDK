// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Sample code for the tutorial at https://dolittle.io/tutorials/getting-started/csharp/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Dolittle.SDK;
using Dolittle.SDK.Events.Store;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder()
    .UseDolittle()
    .Build();

await host.StartAsync();

var client = await host.GetDolittleClient();

await client.Aggregates
    .ForTenant(TenantId.Development)
    .Get<Kitchen>("Dolittle Tacos_2")
    .Perform(kitchen => kitchen.PrepareDish("Bean Blaster Taco", "Mr. Taco"));

// await client.Aggregates
//     .ForTenant(TenantId.Development)
//     .Get<Kitchen>("Dolittle Tacos")
//     .Perform(kitchen => kitchen.PrepareDish("Bean Buster Taco", "Mr. Taco"));




var tacoVersion = 0;

var count = 1;

var parallel = true;

while (true)
{
    var timer = Stopwatch.StartNew();
    var tasks = new List<Task>();

    if (parallel)
    {
        for (var i = 0; i < count; i++)
        {
            tasks.Add(client.Aggregates
                .ForTenant(TenantId.Development)
                .Get<Kitchen>("Dolittle Tacos" + i)
                .Perform(kitchen => kitchen.PrepareDish("Bean Blaster Tacos_"  + tacoVersion++, "Mr. Taco")));
        }
    }
    else
    {
        var root = client.Aggregates
            .ForTenant(TenantId.Development)
            .Get<Kitchen>("Dolittle Tacos");
        for (var i = 0; i < count; i++)
        {
            await root.Perform(kitchen => kitchen.PrepareDish("Bean Blaster Taco_" + tacoVersion++, "Mr. Taco"));
        }
    }


    await Task.WhenAll(tasks);

    Console.WriteLine($"{(parallel ? "Parallelly" : "Sequentially")} committed {count} msg in {timer.Elapsed}, {count / timer.Elapsed.TotalSeconds} / s");


    Console.WriteLine("Next iteration?");
    var line = Console.ReadLine();
    if (line?.Length > 0 && !int.TryParse(line, out count))
    {
        break;
    }

    parallel = !parallel;
}






await host.WaitForShutdownAsync();

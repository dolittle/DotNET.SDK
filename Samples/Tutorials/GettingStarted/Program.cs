// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Sample code for the tutorial at https://dolittle.io/tutorials/getting-started/csharp/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Server.Tracing;
using Dolittle.SDK;
using Dolittle.SDK.Events.Store;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder()
    .UseDolittle()
    .ConfigureServices(services => services.AddTracing())
    .Build();

await host.StartAsync();

var client = await host.GetDolittleClient();

var eventStore = client.EventStore.ForTenant(TenantId.Development);

var tacoVersion = 0;

var count = 1;

var batch = true;

while (true)
{
    var timer = Stopwatch.StartNew();
    var tasks = new List<Task<CommittedEvents>>();

    if (batch)
    {
        tasks.Add(eventStore.Commit(builder =>
        {
            for (var i = 0; i < count; i++)
            {
                builder.CreateEvent(new DishPrepared("Bean Blaster Taco_v_" + tacoVersion++, "Mr. Taco")).FromEventSource("Dolittle Tacos");
            }
        }));
    }
    else
    {
        for (var i = 0; i < count; i++)
        {
            tasks.Add(eventStore.CommitEvent(new DishPrepared("Bean Blaster Taco_v_" + tacoVersion++, "Mr. Taco"), "Dolittle Tacos"));
        }
    }

    for (var i = 0; i < count; i++)
    {
        tasks.Add(eventStore.CommitEvent(new DishPrepared("Bean Blaster Taco_v_" + tacoVersion++, "Mr. Taco"), "Dolittle Tacos"));
    }

    var committedEventsArray = await Task.WhenAll(tasks);

    foreach (var committedEvents in committedEventsArray)
    {
        if (!committedEvents.HasEvents)
        {
            Console.WriteLine("Missing events" + committedEvents.Count);
        }
    }

    Console.WriteLine($"{(batch ? "Batch" : "Individually")} committed {count} msg in {timer.Elapsed}, {count / timer.Elapsed.TotalSeconds} / s");


    Console.WriteLine("Next iteration?");
    var line = Console.ReadLine();
    if (line?.Length > 0 && !int.TryParse(line, out count))
    {
        break;
    }

    batch = !batch;
}


await host.WaitForShutdownAsync();

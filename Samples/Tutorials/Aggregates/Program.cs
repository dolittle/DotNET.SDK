// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Sample code for the tutorial at https://dolittle.io/tutorials/getting-started/csharp/

using System.Threading.Tasks;
using Dolittle.SDK;
using Dolittle.SDK.Tenancy;

namespace Kitchen
{
    class Program
    {
        public static async Task Main()
        {
            var client = DolittleClient
                .ForMicroservice("f39b1f61-d360-4675-b859-53c05c87c0e6")
                .WithEventTypes(eventTypes =>
                    eventTypes.Register<DishPrepared>())
                .WithEventHandlers(builder =>
                    builder.RegisterEventHandler<DishHandler>())
                .WithAggregateRoots(builder =>
                    builder.Register<Kitchen>())
                .Build();

            await client
                .AggregateOf<Kitchen>("bfe6f6e4-ada2-4344-8a3b-65a3e1fe16e9", _ => _.ForTenant(TenantId.Development))
                .Perform(kitchen => kitchen.PrepareDish("Bean Blaster Taco", "Mr. Taco"));

            // Blocks until the EventHandlers are finished, i.e. forever
            client.Start().Wait();
        }
    }
}

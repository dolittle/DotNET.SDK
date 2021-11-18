// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK;
using System.Threading.Tasks;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Filters;
using Dolittle.SDK.Tenancy;

namespace EventHorizon.Producer
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = DolittleClient.ForMicroservice("f39b1f61-d360-4675-b859-53c05c87c0e6")
                .WithEventTypes(eventTypes =>
                    eventTypes.Register<DishPrepared>())
                .WithEventHandlers(eventHandlersBuilder =>
                    eventHandlersBuilder.RegisterEventHandler<DishHandler>())
                .WithFilters(filtersBuilder =>
                    filtersBuilder
                        .CreatePublicFilter("2c087657-b318-40b1-ae92-a400de44e507", filterBuilder =>
                            filterBuilder.Handle((@event, eventContext) =>
                            {
                                Console.WriteLine($"Filtering event {@event} to public streams");
                                return Task.FromResult(new PartitionedFilterResult(true, PartitionId.Unspecified));
                            })))
                .Build();

            var preparedTaco = new DishPrepared("Bean Blaster Taco", "Mr. Taco");

            client.EventStore
                .ForTenant(TenantId.Development)
                .CommitPublicEvent(preparedTaco, "bfe6f6e4-ada2-4344-8a3b-65a3e1fe16e9")
                .GetAwaiter().GetResult();

            // Blocks until the EventHandlers are finished, i.e. forever
            client.Start().Wait();
        }
    }
}

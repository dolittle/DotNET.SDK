// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK;
using Dolittle.SDK.Events;
using Dolittle.SDK.Tenancy;

namespace EventHorizon.Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = Client.ForMicroservice("a14bb24e-51f3-4d83-9eba-44c4cffe6bb9")
                .WithRuntimeOn("localhost", 50055)
                .WithEventTypes(eventTypes =>
                    eventTypes.Register<DishPrepared>())
                .WithEventHorizons(eventHorizons =>
                    eventHorizons.ForTenant(TenantId.Development, subscriptions =>
                        subscriptions
                            .FromProducerMicroservice("f39b1f61-d360-4675-b859-53c05c87c0e6")
                            .FromProducerTenant(TenantId.Development)
                            .FromProducerStream("2c087657-b318-40b1-ae92-a400de44e507")
                            .FromProducerPartition(PartitionId.Unspecified)
                            .ToScope("808ddde4-c937-4f5c-9dc2-140580f6919e")))
                .WithEventHandlers(_ =>
                    _.CreateEventHandler("6c3d358f-3ecc-4c92-a91e-5fc34cacf27e")
                        .InScope("808ddde4-c937-4f5c-9dc2-140580f6919e")
                        .Partitioned()
                        .Handle<DishPrepared>((@event, context) => Console.WriteLine($"Handled event {@event} from public stream")))
                .Build();
            // Blocks until the EventHandlers are finished, i.e. forever
            client.Start().Wait();
        }
    }
}

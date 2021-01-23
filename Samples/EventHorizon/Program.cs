// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.SDK;

namespace EventHorizon
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = Client.ForMicroservice("a14bb24e-51f3-4d83-9eba-44c4cffe6bb9")
                .WithRuntimeOn("localhost", 50055)
                .WithEventTypes(eventTypes =>
                    eventTypes
                        .Associate<MyEvent>("f42529b3-d980-4b55-8fbe-65101a6141a3"))
                .WithEventHandlers(_ =>
                    _.CreateEventHandler("6c3d358f-3ecc-4c92-a91e-5fc34cacf27e")
                        .InScope("808ddde4-c937-4f5c-9dc2-140580f6919e")
                        .Partitioned()
                        .Handle<MyEvent>((@event, context) => Console.WriteLine($"Handled event {@event} with sequence number {context.SequenceNumber}")))
                .WithEventHorizons(eventHorizons => {
                    eventHorizons.ForTenant("900893e7-c4cc-4873-8032-884e965e4b97", subscriptions => {
                        subscriptions
                            .FromProducerMicroservice("7a6155dd-9109-4488-8f6f-c57fe4b65bfb")
                            .FromProducerTenant("900893e7-c4cc-4873-8032-884e965e4b97")
                            .FromProducerStream("2c087657-b318-40b1-ae92-a400de44e507")
                            .FromProducerPartition("00000000-0000-0000-0000-000000000000")
                            .ToScope("808ddde4-c937-4f5c-9dc2-140580f6919e")
                            .OnSuccess((subscriptions, consent) => Console.WriteLine($"Subscription succeeded with {consent}"))
                            .OnFailure((subscriptions, failure) => Console.WriteLine($"Subscription failed with {failure}"))
                            .OnCompleted((response) => Console.WriteLine($"Subscription completed {response}"));
                        subscriptions.OnCompleted((response) => Console.WriteLine($"Completed from tenant {response}"));
                    });
                    eventHorizons.OnCompleted((response) => Console.WriteLine($"Completed from global {response}"));
                })
                .Build();

            client.Start().Wait();
        }
    }
}

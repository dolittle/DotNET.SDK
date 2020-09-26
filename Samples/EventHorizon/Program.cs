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
                .ConnectToRuntime("localhost", 50055)
                .WithEventTypes(eventTypes =>
                    eventTypes
                        .Associate<MyEvent>("f42529b3-d980-4b55-8fbe-65101a6141a3"))
                .WithEventHorizonSubscriptions(eventHorizons => {
                    eventHorizons.ForTenant("900893e7-c4cc-4873-8032-884e965e4b97", tenantSubscriptions => {
                        tenantSubscriptions.FromMicroservice("7a6155dd-9109-4488-8f6f-c57fe4b65bfb", subscriptions => {
                            subscriptions
                                .FromTenant("900893e7-c4cc-4873-8032-884e965e4b97")
                                .FromStream("2c087657-b318-40b1-ae92-a400de44e507")
                                .FromPartition("00000000-0000-0000-0000-000000000000")
                                .ToScope("808ddde4-c937-4f5c-9dc2-140580f6919e");
                        });
                    });
                })
                .Build();

            while (true) Thread.Sleep(1000);
        }
    }
}

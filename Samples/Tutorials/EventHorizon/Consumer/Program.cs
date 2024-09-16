// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.Hosting;

Host.CreateDefaultBuilder()
    .UseDolittle(builder => builder
        .WithEventHorizons(eventHorizon => eventHorizon
            .ForTenant(TenantId.Development, subscriptions => 
                subscriptions
                    .FromProducerMicroservice("f39b1f61-d360-4675-b859-53c05c87c0e6")
                    .FromProducerTenant(TenantId.Development)
                    .FromProducerStream("2c087657-b318-40b1-ae92-a400de44e507")
                    .FromProducerPartition(Guid.Empty)
                    .ToScope("808ddde4-c937-4f5c-9dc2-140580f6919e"))
        )
        .WithEventHandlers(handlers => handlers
            .Create("6c3d358f-3ecc-4c92-a91e-5fc34cacf27e")
            .InScope("808ddde4-c937-4f5c-9dc2-140580f6919e")
            .Partitioned()
            .Handle<DishPrepared>((evt, _) => Console.WriteLine($"Handled event {evt} from public stream"))
        ),
        configuration => configuration.WithRuntimeOn("localhost", 50055))
    .Build()
    .Run();

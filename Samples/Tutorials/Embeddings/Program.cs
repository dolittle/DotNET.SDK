// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Sample code for the tutorial at https://dolittle.io/tutorials/embeddings/

using Dolittle.SDK;
using Dolittle.SDK.Tenancy;

namespace Kitchen
{
    class Program
    {
        public static void Main()
        {
            var client = Client
                .ForMicroservice("f39b1f61-d360-4675-b859-53c05c87c0e6")
                .WithEventTypes(eventTypes =>
                    eventTypes.Register<DishPrepared>())
                .Build();

            var preparedTaco = new DishPrepared("Bean Blaster Taco", "Mr. Taco");

            client.EventStore
                .ForTenant(TenantId.Development)
                .Commit(eventsBuilder =>
                    eventsBuilder
                        .CreateEvent(preparedTaco)
                        .FromEventSource("bfe6f6e4-ada2-4344-8a3b-65a3e1fe16e9"));

            // Blocks until the EventHandlers are finished, i.e. forever
            client.Start().Wait();
        }
    }
}

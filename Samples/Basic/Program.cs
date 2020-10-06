// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK;

namespace Basic
{
    class Program
    {
        public static void Main()
        {
            var client = Client
                .ForMicroservice("f39b1f61-d360-4675-b859-53c05c87c0e6")
                .WithEventTypes(eventTypes =>
                    eventTypes.Register<DishPrepared>())
                .WithEventHandlers(builder =>
                    builder.RegisterEventHandler<DishHandler>())
                .Build();

            var preparedTaco = new DishPrepared
            {
                Dish = "Bean Blaster Taco",
                Chef = "Mr. Taco"
            };

            client.EventStore
                .ForTenant("900893e7-c4cc-4873-8032-884e965e4b97")
                .Commit(preparedTaco, "bfe6f6e4-ada2-4344-8a3b-65a3e1fe16e9");

            // blocks the thread to let all the event handlers to keep handling events
            client.Wait();
        }
    }
}

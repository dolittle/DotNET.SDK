// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Sample code for the tutorial at https://dolittle.io/tutorial/getting-started/csharp/

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

            var preparedTaco = new DishPrepared("Bean Blaster Taco", "Mr. Taco");

            client.EventStore
                .ForTenant("900893e7-c4cc-4873-8032-884e965e4b97")
                .Commit(preparedTaco, "bfe6f6e4-ada2-4344-8a3b-65a3e1fe16e9");

            // blocks the thread to let all the event handlers to keep handling events
            client.Wait();
        }
    }
}

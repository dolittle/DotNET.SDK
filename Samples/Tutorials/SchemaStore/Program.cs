// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Sample code for the tutorial at https://dolittle.io/tutorials/getting-started/csharp/

using System;
using Dolittle.SDK;
using Dolittle.SDK.Tenancy;
// using QuickType;

namespace Kitchen
{
    class Program
    {
        public static void Main()
        {
            // var client = Client
            //     .ForMicroservice("f39b1f61-d360-4675-b859-53c05c87c0e6")
            //     .WithEventTypes(eventTypes =>
            //     {
            //         eventTypes.Register<DishPrepared>();
            //         eventTypes.Register<QuickType.DishPrepared>();
            //     })
            //     .WithEventHandlers(builder =>
            //         builder.RegisterEventHandler<DishHandler>())
            //     .Build();

            // client.Start().Wait();

            var jsonPrepared = new DishPrepared();
            Console.WriteLine($"Prepared be: {jsonPrepared.EventTypeId}");
        }
    }
}

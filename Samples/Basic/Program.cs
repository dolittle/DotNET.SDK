// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK;
using System.Threading;
using System.Threading.Tasks;
using System.Reactive.Linq;

namespace Basic
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var client = Client.ForMicroservice("7a6155dd-9109-4488-8f6f-c57fe4b65bfb")
                .WithEventTypes(eventTypes =>
                    eventTypes
                        .Associate<MyEvent>("f42529b3-d980-4b55-8fbe-65101a6141a3"))
                .WithFilters(filtersBuilder =>
                    filtersBuilder
                        .CreatePrivateFilter("577b00c4-8b79-4727-835c-4710919c2df5", filterBuilder =>
                            filterBuilder.Handle((@event, eventContext) => 
                            {
                                Console.WriteLine($"Filtering event {@event} {eventContext}");
                                return Task.FromResult(true);
                            })))
                .Build();
            client.ExecutionContextManager.ForTenant("900893e7-c4cc-4873-8032-884e965e4b97");

            var myEvent = new MyEvent("test string", 12345);

            var myEventTask = client.EventStore.Commit(myEvent, "8ac5b16a-0b88-4578-a005-e5247c611777");
            Console.WriteLine(myEventTask.Result.Events);

            while (true)
            {
                Thread.Sleep(1000);
            }
        }
    }
}

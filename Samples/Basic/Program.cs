// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Filters;

namespace Basic
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = Client.ForMicroservice("7a6155dd-9109-4488-8f6f-c57fe4b65bfb")
                .WithEventTypes(eventTypes =>
                    eventTypes
                        .Associate<MyEvent>("f42529b3-d980-4b55-8fbe-65101a6141a3"))
                .WithFilters(filtersBuilder =>
                    filtersBuilder
                        .CreatePublicFilter("2c087657-b318-40b1-ae92-a400de44e507", filterBuilder =>
                            filterBuilder.Handle((@event, eventContext) =>
                            {
                                Console.WriteLine($"Filtering event {@event} {eventContext}");
                                return Task.FromResult(new PartitionedFilterResult(true, PartitionId.Unspecified));
                            })))
                .WithEventHandlers(eventHandlersBuilder =>
                    eventHandlersBuilder.CreateEventHandler("e8e53f11-d843-4a77-92dd-f8675ebf6aa0", eventHandlerBuilder =>
                        eventHandlerBuilder
                            .Partitioned()
                            .Handle(async (MyEvent @event, EventContext context) => Console.WriteLine($"Handling event {@event} in first method"))))
                .Build();

            var myEvent = new MyEvent("test string", 12345);
            var commit = client.EventStore.ForTenant("900893e7-c4cc-4873-8032-884e965e4b97").CommitPublic(myEvent, "8ac5b16a-0b88-4578-a005-e5247c611777");
            Console.WriteLine(commit.Result.Events);

            while (true) Thread.Sleep(1000);
        }
    }
}

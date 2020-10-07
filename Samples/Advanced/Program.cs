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
            Console.WriteLine("Hello World!");
            var client = Client.ForMicroservice("7a6155dd-9109-4488-8f6f-c57fe4b65bfb")
                .WithEventTypes(eventTypes =>
                    eventTypes
                        .Register<MyEvent>())
                .WithFilters(filtersBuilder =>
                    filtersBuilder
                        .CreatePublicFilter("2c087657-b318-40b1-ae92-a400de44e507", filterBuilder =>
                            filterBuilder.Handle((@event, eventContext) =>
                            {
                                Console.WriteLine($"Filtering event {@event} {eventContext}");
                                return Task.FromResult(new PartitionedFilterResult(true, PartitionId.Unspecified));
                            })))
                .WithEventHandlers(eventHandlersBuilder =>
                    eventHandlersBuilder
                        .RegisterEventHandler<MyEventHandler>())
                .Build();

            var myEvent = new MyEvent("test string", 12345);
            var commit = client.EventStore
                .ForTenant("900893e7-c4cc-4873-8032-884e965e4b97")
                .CommitPublic(myEvent, "8ac5b16a-0b88-4578-a005-e5247c611777");

            var aggregateCommit = client.EventStore
                .ForTenant("900893e7-c4cc-4873-8032-884e965e4b97")
                .CommitForAggregate(myEvent, "b2410210-0ae3-4539-aada-fc962bdd1303", "4a5e4db0-84e6-409d-97cc-dc926f48563d", 0);
            Console.WriteLine(commit.Result.Events);
            Console.WriteLine(aggregateCommit.Result.Events);
            Console.WriteLine(aggregateCommit.Result.Failure);

            var fetch = client.EventStore
                .ForTenant("900893e7-c4cc-4873-8032-884e965e4b97")
                .FetchForAggregate("4a5e4db0-84e6-409d-97cc-dc926f48563d", "b2410210-0ae3-4539-aada-fc962bdd1303");
            Console.WriteLine(fetch.Result.Events.Count);

            client.Wait();
        }
    }
}

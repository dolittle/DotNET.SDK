// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Filters;
using Dolittle.SDK.Tenancy;

namespace Basic
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var client = Client.ForMicroservice("f39b1f61-d360-4675-b859-53c05c87c0e6")
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
            // var commit = client.EventStore
            //     .ForTenant(TenantId.Development)
            //     .Commit(eventsBuilder =>
            //         eventsBuilder
            //             .CreatePublicEvent(myEvent)
            //             .FromEventSource("8ac5b16a-0b88-4578-a005-e5247c611777"));

            while (!System.Diagnostics.Debugger.IsAttached) System.Threading.Thread.Sleep(50);

            var committedAggregateEvents = client.EventStore
                .ForTenant(TenantId.Development)
                .ForAggregate("b2410210-0ae3-4539-aada-fc962bdd1303")
                .WithEventSource("8ac5b16a-0b88-4578-a005-e5247c611777")
                .ExpectVersion(0)
                .Commit(eventsBuilder => eventsBuilder.CreateEvent(myEvent))
                .GetAwaiter().GetResult();

            Console.WriteLine(committedAggregateEvents.AggregateRootVersion);
            var fetch = client.EventStore
                .ForTenant(TenantId.Development)
                .FetchForAggregate("b2410210-0ae3-4539-aada-fc962bdd1303", "8ac5b16a-0b88-4578-a005-e5247c611777");
            Console.WriteLine(fetch.Result.Count);

            client.Wait();
        }
    }
}

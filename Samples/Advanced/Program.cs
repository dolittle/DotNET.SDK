// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK;
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
            var client = Client.ForMicroservice("7a6155dd-9109-4488-8f6f-c57fe4b65bfb")
                .WithEventTypes(eventTypes =>
                {
                    eventTypes.Register<MyEvent>();
                    eventTypes.Register<MyOtherEvent>();
                })
                .WithFilters(filtersBuilder =>
                    filtersBuilder
                        .CreatePublicFilter("2c087657-b318-40b1-ae92-a400de44e507", filterBuilder =>
                            filterBuilder.Handle((@event, eventContext) =>
                            {
                                Console.WriteLine($"Filtering event {@event} {eventContext}");
                                return Task.FromResult(new PartitionedFilterResult(true, PartitionId.Unspecified));
                            })))
                .WithEventHandlers(eventHandlersBuilder =>
                {
                    eventHandlersBuilder.RegisterEventHandler<MyEventHandler>();
                    
                    eventHandlersBuilder.CreateEventHandler("44ac0f75-9f13-45f1-b3b1-3f222f92ca57")
                        .Partitioned()
                        .Handle<MyEvent>((@event, ctx) => Console.WriteLine("A second handle method for MyEvent."))
                        .Handle<MyOtherEvent>((@event, ctx) => Console.WriteLine("A handle method for MyOtherevent."));
                })
                .Build();

            var myEvent = new MyEvent("test string", 12345);
            var commit = client.EventStore.ForTenant(TenantId.Development).CommitPublic(myEvent, "8ac5b16a-0b88-4578-a005-e5247c611777");
            Console.WriteLine(commit.Result.Events);
            var myOtherEvent = new MyOtherEvent("other test stering", 54321);
            var otherCommit = client.EventStore.ForTenant(TenantId.Development).CommitPublic(myOtherEvent, "8ac5b16a-0b88-4578-a005-e5247c611777");
            Console.WriteLine(otherCommit.Result.Events);

            client.Start();
        }
    }
}

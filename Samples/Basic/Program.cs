// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Events;

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
                        .Associate<MyEvent>())
                .WithEventHandlers(eventHandlersBuilder =>
                    eventHandlersBuilder
                        .RegisterEventHandler<MyEventHandler>())
                .Build();

            var myEvent = new MyEvent("test string", 12345);
            var commit = client.EventStore.ForTenant("900893e7-c4cc-4873-8032-884e965e4b97").Commit(myEvent, "8ac5b16a-0b88-4578-a005-e5247c611777");
            Console.WriteLine(commit.Result.Events);

            while (true) Thread.Sleep(1000);
        }
    }
}

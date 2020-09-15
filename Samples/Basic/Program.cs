// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK;

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
                        .Associate<MyEvent>("f42529b3-d980-4b55-8fbe-65101a6141a3")
                        .Associate<MyOtherEvent>("4463a85f-2e82-400b-ac49-9e795f6b4f06"))
                .Build();
            client.ExecutionContextManager.ForTenant("900893e7-c4cc-4873-8032-884e965e4b97");

            var myEvent = new MyEvent("MyString", 12345);

            var task = client.EventStore.Commit(myEvent, "35901e38-55fb-4cd8-9d49-bfe556ea0030");
            task.Wait();
            Console.WriteLine(task.Result);
        }
    }
}

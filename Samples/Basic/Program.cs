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
            var client = Client.ForMicroservice("e59582cb-e51d-408e-8f2d-b5e47ed08d65")
                .WithEventTypes(eventTypes =>
                    eventTypes
                        .Associate<MyEvent>("f42529b3-d980-4b55-8fbe-65101a6141a3")
                        .Associate<MyOtherEvent>("4463a85f-2e82-400b-ac49-9e795f6b4f06"))
                .Build();
            client.ExecutionContextManager.ForTenant("546fcbec-8167-41a0-b865-1d881e6efe9e");
        }
    }
}

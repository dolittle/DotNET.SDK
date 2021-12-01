// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Sample code for the tutorial at https://dolittle.io/tutorials/embeddings/

using System;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.SDK;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder()
    .UseDolittle(clientBuilder => clientBuilder
        .WithEventTypes(eventTypes => eventTypes
            .Register<EmployeeHired>()
            .Register<EmployeeTransferred>()
            .Register<EmployeeRetired>())
        .WithEmbeddings(embeddings => embeddings.RegisterEmbedding<Employee>()))
    .Build();

await host.StartAsync();

var client = await host.GetDolittleClient();
// Mock of the state from the external HR system
var updatedEmployee = new Employee
{
    Name = "Mr. Taco",
    Workplace = "Street Food Taco Truck"
};
// Wait for embedding to be registered in the Runtime
await Task.Delay(TimeSpan.FromSeconds(2));

await client.Embeddings
    .ForTenant(TenantId.Development)
    .Update(updatedEmployee.Name, updatedEmployee);
Console.WriteLine($"Updated {updatedEmployee.Name}.");
var mrTaco = await client.Embeddings
    .ForTenant(TenantId.Development)
    .Get<Employee>("Mr. Taco");
Console.WriteLine($"Mr. Taco is now working at {mrTaco.State.Workplace}");

var allEmployeeNames = await client.Embeddings
    .ForTenant(TenantId.Development)
    .GetKeys<Employee>();
Console.WriteLine($"All current employees are {string.Join(",", allEmployeeNames)}");

await client.Embeddings
    .ForTenant(TenantId.Development)
    .Delete<Employee>(updatedEmployee.Name);
Console.WriteLine($"Deleted {updatedEmployee.Name}.");

await host.WaitForShutdownAsync();

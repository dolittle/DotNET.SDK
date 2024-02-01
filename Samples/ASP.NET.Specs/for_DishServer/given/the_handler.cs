// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Customers;
using Dolittle.SDK.Events;
using Dolittle.SDK.Testing.Aggregates;
using Kitchen;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace Specs.for_DishServer.given;

public class the_handler
{
    protected readonly EventSourceId customer_name = DishServer.CustomerName;
    protected DishServer handler;
    protected AggregateOfMock<Customer> customers;
    protected EventSourceId kitchen_name;
    protected DishPrepared @event;
    
    protected the_handler()
    {
        customers = AggregateOfMock<Customer>.Create(_ => _.AddLogging());
        handler = new DishServer(customers, NullLogger<DishServer>.Instance);
        kitchen_name = "Dolittle Tacos";
        @event = new DishPrepared("some dish", "some chef");
    }
}

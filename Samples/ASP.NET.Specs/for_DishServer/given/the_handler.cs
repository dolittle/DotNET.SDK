// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Customers;
using Dolittle.SDK.Events;
using Dolittle.SDK.Testing.Aggregates;
using Kitchen;
using Machine.Specifications;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace Specs.for_DishServer.given;

class the_handler
{
    protected static readonly EventSourceId customer_name = DishServer.CustomerName; 
    protected static DishServer handler;
    protected static AggregateOfMock<Customer> customers;
    protected static EventSourceId kitchen_name;
    protected static DishPrepared @event;
    
    Establish context = () =>
    {
        customers = AggregateOfMock<Customer>.Create(_ => _.AddLogging());
        handler = new DishServer(customers, NullLogger<DishServer>.Instance);
        kitchen_name = "Dolittle Tacos";
        @event = new DishPrepared("some dish", "some chef");
    };
}

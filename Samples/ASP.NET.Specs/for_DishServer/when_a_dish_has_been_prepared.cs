// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Customers;
using Kitchen;
using Machine.Specifications;

namespace Specs.for_DishServer;

class when_a_dish_has_been_prepared : given.the_handler
{
    Because of = async () => await handler.Handle(@event, Specs.given.an_event_context_for<DishPrepared>(kitchen_name));

    It should_get_the_correct_customer = () => customers.TryGetAggregate(customer_name, out _).ShouldBeTrue(); 
    
    It the_customer_should_have_eaten_the_dish_once = () => customers.AssertThat(customer_name).ShouldHaveEvent<DishEaten>()
        .CountOf(1)
        .First().Where(_ => _.Dish.ShouldEqual(@event.Dish));

    It the_customer_should_have_only_eaten_one_dish = () => customers.AssertThat(customer_name).ShouldHaveNumberOfEvents(1);
    
}
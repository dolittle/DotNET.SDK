// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Customers;
using Kitchen;
using Machine.Specifications;

namespace Specs.for_DishServer;

class when_two_dishes_have_been_prepared : given.the_handler
{
    static DishPrepared first_dish, second_dish;
    
    Establish context = () =>
    {
        first_dish = @event with
        {
            Dish = "first dish",
            Chef = "first chef"
        };
        second_dish = @event with
        {
            Dish = "second dish",
            Chef = "second chef"
        };

        handler.Handle(first_dish, Specs.given.an_event_context_for<DishPrepared>(kitchen_name)).GetAwaiter().GetResult();
    };
    
    Because of = async () => await handler.Handle(second_dish, Specs.given.an_event_context_for<DishPrepared>(kitchen_name));

    It should_get_the_correct_customer = () => customers.TryGetAggregate(customer_name, out _).ShouldBeTrue(); 
    
    It the_customer_should_have_only_eaten_second_dish_after_last_operation = () => customers.AfterLastOperationOn(customer_name).ShouldHaveEvent<DishEaten>()
        .AndThat(_ => _.Dish.ShouldEqual(second_dish.Dish));

    It the_customer_should_have_only_eaten_one_dish_after_last_operation = () => customers.AfterLastOperationOn(customer_name).ShouldHaveNumberOfEvents(1);

    It the_customer_should_have_eaten_first_dish_first = () => customers.AssertThat(customer_name).ShouldHaveEvents<DishEaten>()
        .AtBeginning()
        .AndThat(_ => _.Dish.ShouldEqual(first_dish.Dish));
    
    It the_customer_should_have_eaten_second_dish_last = () => customers.AssertThat(customer_name).ShouldHaveEvents<DishEaten>()
        .AtEnd()
        .AndThat(_ => _.Dish.ShouldEqual(second_dish.Dish));

    It the_customer_should_have_only_eaten_two_dishes = () => customers.AssertThat(customer_name).ShouldHaveNumberOfEvents(2);
    
}
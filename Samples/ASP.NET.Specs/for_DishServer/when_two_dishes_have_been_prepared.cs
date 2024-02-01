// Copyright (c) Dolittle. All rights reserved.
// Licensed under the M[Fact] public void license. See LICENSE file in the project root for full license information.

using Customers;
using FluentAssertions;
using Kitchen;
using Xunit;

namespace Specs.for_DishServer;

public class when_two_dishes_have_been_prepared : given.the_handler
{
    DishPrepared first_dish, second_dish;

    public when_two_dishes_have_been_prepared()
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

        handler.Handle(second_dish, Specs.given.an_event_context_for<DishPrepared>(kitchen_name)).GetAwaiter().GetResult();
    }


    [Fact]
    public void should_get_the_correct_customer() => customers.TryGetAggregate(customer_name, out _).Should().BeTrue();

    [Fact]
    public void the_customer_should_have_only_eaten_second_dish_after_last_operation() => customers
        .AfterLastOperationOn(customer_name)
        .ShouldHaveSingleEvent<DishEaten>()
        .Where(_ => _.Dish.Should().Be(second_dish.Dish));

    [Fact]
    public void the_customer_should_have_only_eaten_one_dish_after_last_operation() =>
        customers.AfterLastOperationOn(customer_name).ShouldHaveNumberOfEvents(1);

    [Fact]
    public void the_customer_should_have_eaten_first_dish_first() => customers.AssertThat(customer_name).ShouldHaveEvent<DishEaten>()
        .AtBeginning()
        .Where(_ => _.Dish.Should().Be(first_dish.Dish));

    [Fact]
    public void the_customer_should_have_eaten_second_dish_last() => customers.AssertThat(customer_name).ShouldHaveEvent<DishEaten>()
        .AtEnd()
        .Where(_ => _.Dish.Should().Be(second_dish.Dish));

    [Fact]
    public void the_customer_should_have_only_eaten_two_dishes() => customers.AssertThat(customer_name).ShouldHaveNumberOfEvents(2);
}
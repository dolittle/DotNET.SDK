// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Customers;
using FluentAssertions;
using Kitchen;
using Xunit;

namespace Specs.for_DishServer;

public class when_a_dish_has_been_prepared : given.the_handler
{
    public when_a_dish_has_been_prepared()
    {
        handler.Handle(@event, Specs.given.an_event_context_for<DishPrepared>(kitchen_name)).GetAwaiter().GetResult();
    }
    
    [Fact]
    public void should_get_the_correct_customer()
    {
        customers.TryGetAggregate(customer_name, out _).Should().BeTrue();
    }
    
    [Fact]
    public void should_have_eaten_the_dish_once()
    {
        customers.AssertThat(customer_name).ShouldHaveEvent<DishEaten>()
            .CountOf(1)
            .First().Where(_ => _.Dish.Should().Be(@event.Dish));
    }

    [Fact]
    public void the_customer_should_have_only_eaten_one_dish()
    {
        customers.AssertThat(customer_name).ShouldHaveNumberOfEvents(1);
    }
}
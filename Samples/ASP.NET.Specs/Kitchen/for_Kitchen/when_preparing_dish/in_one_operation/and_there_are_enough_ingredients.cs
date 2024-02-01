// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentAssertions;
using Kitchen;
using Xunit;

namespace Specs.Kitchen.for_Kitchen.when_preparing_dish.in_one_operation;

public class and_there_are_enough_ingredients : given.a_kitchen
{
    string chef, dish;

    public and_there_are_enough_ingredients()
    {
        chef = "Sindre";
        dish = "Tacos";

        WithAggregateInState(kitchen =>
        {
            kitchen.PrepareDish(chef, dish);
        });
    }

    [Fact]
    public void should_prepare_dish()
    {
        WhenPerforming(kitchen =>
        {
            kitchen.PrepareDish(chef, dish);
        });
        
        AssertThat
            .ShouldHaveEvent<DishPrepared>()
            .CountOf(1)
            .WhereAll(
                evt => evt.Chef.Should().Be(chef),
                evt => evt.Dish.Should().Be(dish));
    }
}
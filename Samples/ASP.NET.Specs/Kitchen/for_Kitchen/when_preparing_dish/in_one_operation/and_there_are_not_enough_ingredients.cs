// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentAssertions;
using Kitchen;
using Xunit;

namespace Specs.Kitchen.for_Kitchen.when_preparing_dish.in_one_operation;

public class and_there_are_not_enough_ingredients : given.a_kitchen
{
    public and_there_are_not_enough_ingredients()
    {
        WithAggregateInState(kitchen =>
        {
            kitchen.PrepareDish("chef", "dish");
            kitchen.PrepareDish("chef", "dish");
        });
    }

    [Fact]
    public void should_fail_if_preparing_dish() =>
        WhenPerforming(kitchen => kitchen.Invoking(
                it => it.PrepareDish("chef", "dish")
            )
            .Should().Throw<OutOfIngredients>());
}
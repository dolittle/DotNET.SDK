// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Dolittle.SDK.Aggregates;
using Kitchen;
using Machine.Specifications;

namespace Specs.Kitchen.for_Kitchen.when_preparing_dish;

class and_there_are_not_enough_ingredients : given.a_kitchen_with_no_prepared_dishes
{
    static Exception failure; 
    Establish context = () =>
    {
        var num_ingredients = 2;
        foreach (var _ in Enumerable.Range(0, num_ingredients))
        {
            OnKitchen().PerformSync(_ => _.PrepareDish("some chef", "some dish"));
        }
    };

    Because of = async () => failure = await Catch.ExceptionAsync(() => OnKitchen().Perform(_ => _.PrepareDish("chef", "dish")));

    It should_after_last_operation_not_have_done_anything = () => KitchenAfterLastOperation().ShouldHaveNoEvents();
    
    It should_only_have_prepared_two_dishes_before = () => KitchenWithAllEvents().ShouldHaveEvents<DishPrepared>().CountOf(2);

    It should_have_done_nothing_else = () => KitchenWithAllEvents().ShouldHaveNumberOfEvents(2);

    It should_fail_because_operation_failed = () => failure.ShouldBeOfExactType<AggregateRootOperationFailed>();

}
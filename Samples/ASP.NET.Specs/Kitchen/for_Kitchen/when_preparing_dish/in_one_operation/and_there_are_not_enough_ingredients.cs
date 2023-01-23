// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Dolittle.SDK.Aggregates;
using Kitchen;
using Machine.Specifications;

namespace Specs.Kitchen.for_Kitchen.when_preparing_dish.in_one_operation;

class and_there_are_not_enough_ingredients : given.a_kitchen_with_no_prepared_dishes
{
    static Exception failure;

    Because of = async () => failure = await Catch.ExceptionAsync(() => OnKitchen().Perform(customer =>
    {
        var num_ingredients = 2;
        foreach (var _ in Enumerable.Range(0, num_ingredients + 1))
        {
            customer.PrepareDish("chef", "dish");
        }
    }));

    It should_after_last_operation_not_have_done_anything = () => KitchenAfterLastOperation().ShouldHaveNoEvents();

    It should_only_not_have_done_anything_at_all = () => KitchenWithAllEvents().ShouldHaveNoEvents();

    It should_fail_because_operation_failed = () => failure.ShouldBeOfExactType<AggregateRootOperationFailed>();

}
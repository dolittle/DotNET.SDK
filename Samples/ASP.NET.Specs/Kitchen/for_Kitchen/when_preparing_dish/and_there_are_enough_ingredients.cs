// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Kitchen;
using Machine.Specifications;

namespace Specs.Kitchen.for_Kitchen.when_preparing_dish;

class and_there_are_enough_ingredients : given.a_kitchen_with_no_prepared_dishes
{
    static string chef, dish;
    Establish context = () =>
    {
        chef = "Sindre";
        dish = "Tacos";
    };

    Because of = async () => await OnKitchen().Perform(_ => _.PrepareDish(chef, dish));
    
    It should_only_prepare_one_dish = () => KitchenWithAllEvents().ShouldHaveEvent<DishPrepared>()
        .AndThat(
            _ => _.Chef.ShouldEqual(chef),
            _ => _.Dish.ShouldEqual(dish));

    It should_have_done_nothing_else = () => KitchenWithAllEvents().ShouldHaveNumberOfEvents(1);

}
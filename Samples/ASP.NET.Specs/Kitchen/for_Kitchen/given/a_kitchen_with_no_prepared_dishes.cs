// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;
using Dolittle.SDK.Testing.Aggregates;
using Machine.Specifications;

namespace Specs.Kitchen.for_Kitchen.given;

class a_kitchen_with_no_prepared_dishes
{
    protected static AggregateOfMock<global::Kitchen.Kitchen> kitchens;
    protected static EventSourceId kitchen_name;
    
    Establish context = () =>
    {
        kitchens = AggregateOfMock<global::Kitchen.Kitchen>.Create();
        kitchen_name = "Dolittle Tacos";
    };

    protected static AggregateRootOperationsMock<global::Kitchen.Kitchen> OnKitchen() => kitchens.GetMock(kitchen_name);
    protected static AggregateRootAssertion KitchenAfterLastOperation() => kitchens.AfterLastOperationOn(kitchen_name);
    protected static AggregateRootAssertion KitchenWithAllEvents() => kitchens.AssertThat(kitchen_name);
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Validation.for_DynamicState
{
    public class when_getting_member_from_model
    {
        static Model model;
        static dynamic state;
        static string result;

        Establish context = () =>
        {
            model = new Model();
            state = new DynamicState(model, new[] { Model.TheStringProperty });
        };

        Because of = () => result = state.TheString;

        It should_call_get_on_model = () => model.TheStringGetCalled.ShouldBeTrue();
    }
}

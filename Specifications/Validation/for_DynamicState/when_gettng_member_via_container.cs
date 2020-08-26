// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Validation.for_DynamicState
{
    public class when_gettng_member_via_container
    {
        static ModelContainer container;
        static Model model;
        static dynamic state;
        static string result;

        Establish context = () =>
        {
            container = new ModelContainer();
            model = new Model();
            container.Model = model;
            state = new DynamicState(container, new[] { Model.TheStringProperty });
        };

        Because of = () => result = state.TheString;

        It should_call_get_on_model = () => model.TheStringGetCalled.ShouldBeTrue();
    }
}

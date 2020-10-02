// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.SDK.Concepts.for_Value
{
    [Subject(typeof(Value<>))]
    public class when_equating_two_instances_with_different_values : given.value_objects
    {
        static bool is_equal;

        Because of = () => is_equal = first_value.Equals(different_value);

        It should_not_be_equal = () => is_equal.ShouldBeFalse();
    }
}
﻿// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.SDK.Concepts.for_Value
{
    [Subject(typeof(Value<>))]
    public class when_equating_two_instances_with_identical_values : given.value_objects
    {
        static bool is_equal;

        Because of = () => is_equal = first_value.Equals(identical_values_to_first_value);

        It should_be_equal = () => is_equal.ShouldBeTrue();
    }
}
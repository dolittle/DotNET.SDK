﻿// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.SDK.Concepts.for_Value
{
    public class when_getting_hashcode_for_values_and_inherited_type_with_identical_values : given.value_objects
    {
        static int first_hashcode;
        static int second_hashcode;

        Because of = () =>
            {
                first_hashcode = first_value.GetHashCode();
                second_hashcode = inherited_value_with_identical_values.GetHashCode();
            };

        It should_have_different_hashcodes = () => first_hashcode.ShouldNotEqual(second_hashcode);
    }
}
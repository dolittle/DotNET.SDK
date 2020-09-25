// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.SDK.Concepts.for_ConceptAs
{
    [Subject(typeof(ConceptAs<>))]
    public class when_checking_is_greater_than : given.test_concepts
    {
        static bool least_is_greater_than_most;
        static bool least_is_greater_than_middle;
        static bool least_is_greater_than_self;
        static bool least_is_greater_than_primitive_value;
        static bool middle_is_greater_than_least;
        static bool middle_is_greater_than_most;
        static bool middle_is_greater_than_self;
        static bool middle_is_greater_than_primitive_value;
        static bool most_is_greater_than_least;
        static bool most_is_greater_than_middle;
        static bool most_is_greater_than_self;
        static bool most_is_greater_than_primitive_value;

        Because of = () =>
        {
#pragma warning disable CS1718
            least_is_greater_than_most = least > most;
            least_is_greater_than_middle = least > middle;
            least_is_greater_than_self = least > least;
            least_is_greater_than_primitive_value = least > least.Value;
            middle_is_greater_than_least = middle > least;
            middle_is_greater_than_most = middle > most;
            middle_is_greater_than_self = middle > middle;
            middle_is_greater_than_primitive_value = middle > middle.Value;
            most_is_greater_than_least = most > least;
            most_is_greater_than_middle = most > middle;
            most_is_greater_than_self = most > most;
            most_is_greater_than_primitive_value = most > most.Value;
#pragma warning restore
        };

        It determines_least_is_not_greater_than_most = () => least_is_greater_than_most.ShouldBeFalse();
        It determines_least_is_not_greater_than_middle = () => least_is_greater_than_middle.ShouldBeFalse();
        It determines_least_is_not_greater_than_self = () => least_is_greater_than_self.ShouldBeFalse();
        It determines_least_is_not_greater_than_primitive_value = () => least_is_greater_than_primitive_value.ShouldBeFalse();
        It determines_middle_is_not_greater_than_most = () => middle_is_greater_than_most.ShouldBeFalse();
        It determines_middle_is_greater_than_least = () => middle_is_greater_than_least.ShouldBeTrue();
        It determines_middle_is_not_greater_than_self = () => middle_is_greater_than_self.ShouldBeFalse();
        It determines_middle_is_not_greater_than_primitive_value = () => middle_is_greater_than_primitive_value.ShouldBeFalse();
        It determines_most_is_greater_than_least = () => most_is_greater_than_least.ShouldBeTrue();
        It determines_most_is_greater_than_middle = () => most_is_greater_than_middle.ShouldBeTrue();
        It determines_most_is_not_greater_than_self = () => most_is_greater_than_self.ShouldBeFalse();
        It determines_most_is_not_greater_than_primitive_value = () => most_is_greater_than_primitive_value.ShouldBeFalse();
    }
}
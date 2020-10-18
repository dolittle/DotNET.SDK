// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Machine.Specifications;

namespace Dolittle.SDK.Aggregates.for_AggregateRoot
{
    public class when_getting_on_methods : given.two_aggregate_roots
    {
        static bool got_an_event_method_on_statless;
        static MethodInfo an_event_method_on_statless;
        static bool got_another_event_method_on_statless;
        static MethodInfo another_event_method_on_statless;
        static bool got_an_event_method_on_statfull;
        static MethodInfo an_event_method_on_statfull;
        static bool got_another_event_method_on_statfull;
        static MethodInfo another_event_method_on_statfull;

        Because of = () =>
        {
            got_an_event_method_on_statless = stateless_aggregate_root.TryGetOnMethod(first_event, out an_event_method_on_statless);
            got_another_event_method_on_statless = stateless_aggregate_root.TryGetOnMethod(third_event, out another_event_method_on_statless);
            got_an_event_method_on_statfull = statefull_aggregate_root.TryGetOnMethod(second_event, out an_event_method_on_statfull);
            got_another_event_method_on_statfull = statefull_aggregate_root.TryGetOnMethod(third_event, out another_event_method_on_statfull);
        };

        It should_not_get_for_an_event_on_stateless = () => got_an_event_method_on_statless.ShouldBeFalse();
        It should_be_null_for_an_event_on_stateless = () => an_event_method_on_statless.ShouldBeNull();
        It should_not_get_for_another_event_on_stateless = () => got_another_event_method_on_statless.ShouldBeFalse();
        It should_be_null_for_another_event_on_stateless = () => another_event_method_on_statless.ShouldBeNull();
        It should_get_method_for_on_event_on_statefull = () => got_an_event_method_on_statfull.ShouldBeTrue();
        It should_get_the_correct_method_for_on_event_on_statefull = () => an_event_method_on_statfull.ShouldEqual(statefull_aggregate_root.OnSimpleEventMethod);
        It should_get_method_for_another_event_on_statefull = () => got_another_event_method_on_statfull.ShouldBeTrue();
        It should_get_the_correct_method_for_another_event_on_statefull = () => another_event_method_on_statfull.ShouldEqual(statefull_aggregate_root.OnAnotherEventMethod);
    }
}

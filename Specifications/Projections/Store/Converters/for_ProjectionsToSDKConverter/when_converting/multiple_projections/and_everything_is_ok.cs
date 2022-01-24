// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.SDK.Projections.Store.Converters.for_ProjectionsToSDKConverter.given;
using Machine.Specifications;

namespace Dolittle.SDK.Projections.Store.Converters.for_ProjectionsToSDKConverter.when_converting.multiple_projections;

public class and_everything_is_ok : a_converter
{
    static CurrentState<a_projection_type> first_projection_state;
    static CurrentStateType first_state_type;
    static CurrentState<a_projection_type> second_projection_state;
    static CurrentStateType second_state_type;
    static IEnumerable<CurrentState<a_projection_type>> result_projections;
    static Exception result_exception;
    static bool succeeded;
    
    Establish context = () =>
    {
        first_state_type = CurrentStateType.Persisted;
        second_state_type = CurrentStateType.CreatedFromInitialState;
        first_projection_state = new CurrentState<a_projection_type>(new a_projection_type{AValue = 42}, first_state_type, "first key");
        second_projection_state = new CurrentState<a_projection_type>(new a_projection_type{AValue = 43}, second_state_type, "second key");
    };

    Because of = () => succeeded = converter.TryConvert(
        new []
        {
            create_protobuf_projection_current_state(first_projection_state, first_state_type),
            create_protobuf_projection_current_state(second_projection_state, second_state_type)
        },
        out result_projections,
        out result_exception);

    It should_succeed = () => succeeded.ShouldBeTrue();
    It should_not_return_error = () => result_exception.ShouldBeNull();
    It should_return_projections = () => result_projections.ShouldNotBeNull();
    It should_return_two_projections = () => result_projections.Count().ShouldEqual(2);
    It should_return_projections_with_the_correct_keys = () => result_projections.Select(_ => _.Key).ShouldContainOnly(first_projection_state.Key, second_projection_state.Key);
    It should_return_the_first_projection_with_correct_value = () => result_projections.ShouldContain(_ => _.State.AValue.Equals(first_projection_state.State.AValue) && _.Key == first_projection_state.Key);
    It should_return_the_second_projection_with_correct_value = () => result_projections.ShouldContain(_ => _.State.AValue.Equals(second_projection_state.State.AValue) && _.Key == second_projection_state.Key);
}
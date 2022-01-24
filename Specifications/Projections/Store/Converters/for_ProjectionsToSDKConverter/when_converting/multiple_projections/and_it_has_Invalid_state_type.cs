// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.SDK.Projections.Store.Converters.for_ProjectionsToSDKConverter.given;
using Machine.Specifications;

namespace Dolittle.SDK.Projections.Store.Converters.for_ProjectionsToSDKConverter.when_converting.multiple_projections;

public class and_it_has_Invalid_state_type : a_converter
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
        first_state_type = (CurrentStateType)11;
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

    It should_not_succeed = () => succeeded.ShouldBeFalse();
    It should_not_return_a_projection = () => result_projections.ShouldBeNull();
    It should_fail_because_invalid_current_state_type = () => result_exception.ShouldBeOfExactType<InvalidCurrentStateType>();
}
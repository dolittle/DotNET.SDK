// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Projections.Store.Converters.for_ProjectionsToSDKConverter.given;
using Machine.Specifications;

namespace Dolittle.SDK.Projections.Store.Converters.for_ProjectionsToSDKConverter.when_converting.a_projection;

public class and_projection_type_has_additional_field : a_converter
{
    static a_projection_type_with_additional_fields projection_state;
    static Key key;
    static CurrentStateType current_state_type;
    static CurrentState<a_projection_type> result_projection;
    static Exception result_exception;
    static bool succeeded;
    
    Establish context = () =>
    {
        projection_state = new a_projection_type_with_additional_fields()
        {
            AValue = "hello",
            AnotherValue = 42
        };
        key = "some_key";
        current_state_type = CurrentStateType.Persisted;
    };

    Because of = () => succeeded = converter.TryConvert(
        create_protobuf_projection_current_state(projection_state, key, current_state_type),
        out result_projection,
        out result_exception);

    It should_succeed = () => succeeded.ShouldBeTrue();
    It should_not_return_error = () => result_exception.ShouldBeNull();
    It should_return_a_projection = () => result_projection.ShouldNotBeNull();
    It should_return_a_projection_state_with_correct_key = () => result_projection.Key.ShouldEqual(key);
    It should_return_a_projection_state_with_correct_state_type = () => result_projection.WasCreatedFromInitialState.ShouldBeFalse();
    It should_return_a_projection_with_the_same_value_for_the_common_field = () => result_projection.State.AValue.ShouldEqual(projection_state.AValue);
}
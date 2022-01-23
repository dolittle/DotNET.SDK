// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Projections.Store.Converters.for_ProjectionsToSDKConverter.given;
using Machine.Specifications;

namespace Dolittle.SDK.Projections.Store.Converters.for_ProjectionsToSDKConverter.when_converting.a_projection;

public class and_projection_type_has_field_of_incompatible_type : a_converter
{
    static a_projection_type_with_field_but_different_type projection_state;
    static Key key;
    static CurrentStateType current_state_type;
    static CurrentState<a_projection_type> result_projection;
    static Exception result_exception;
    static bool succeeded;
    
    Establish context = () =>
    {
        projection_state = new a_projection_type_with_field_but_different_type()
        {
            AValue = 42
        };
        key = "some_key";
        current_state_type = CurrentStateType.Persisted;
    };

    Because of = () => succeeded = converter.TryConvert(
        create_protobuf_projection_current_state(projection_state, key, current_state_type),
        out result_projection,
        out result_exception);

    It should_not_succeed = () => succeeded.ShouldBeFalse();
    It should_not_return_a_projection = () => result_projection.ShouldBeNull();
    It should_fail_because_could_not_deserialize_projection = () => result_exception.ShouldBeOfExactType<CouldNotDeserializeProjection>();
}
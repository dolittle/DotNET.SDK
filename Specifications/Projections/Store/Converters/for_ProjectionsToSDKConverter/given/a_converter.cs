// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Projections.Contracts;
using Machine.Specifications;
using Newtonsoft.Json;

namespace Dolittle.SDK.Projections.Store.Converters.for_ProjectionsToSDKConverter.given;

public class a_converter
{
    protected static ProjectionsToSDKConverter converter;
    Establish context = () =>
    {
        converter = new ProjectionsToSDKConverter();
    };

    protected static ProjectionCurrentState create_protobuf_projection_current_state<TReadModel>(TReadModel currentState, Key key, CurrentStateType type)
        where TReadModel : class, new()
        => new()
        {
            State = JsonConvert.SerializeObject(currentState, Formatting.None),
            Key = key,
            Type = get_current_state_type(type)
        };

    static ProjectionCurrentStateType get_current_state_type(CurrentStateType type)
        => type switch
        {
            CurrentStateType.Persisted => ProjectionCurrentStateType.Persisted,
            CurrentStateType.CreatedFromInitialState => ProjectionCurrentStateType.CreatedFromInitialState,
            _ => (ProjectionCurrentStateType) type
        };
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Runtime.Projections.Contracts;
using Machine.Specifications;

namespace Dolittle.SDK.Projections.Store.for_ProjectionStore.when_getting_one_projection.typed;

public class and_it_returns_a_projection_state_another_key : given.all_dependencies
{
    static Key key_in_request;
    static given.a_decorated_projection_type stored_state;
    static ScopedProjectionId scoped_projection_id;
    static Exception result;
    
    Establish context = () =>
    {
        key_in_request = "some key";
        stored_state = new given.a_decorated_projection_type
        {
            Value = 42
        };
        with_projection_types(typeof(given.a_decorated_projection_type));
        get_one_returns("another key", stored_state, ProjectionCurrentStateType.Persisted);
        scoped_projection_id = read_model_types.GetFor<given.a_decorated_projection_type>();
    };

    Because of = () => result = Catch.Exception(() => projection_store.Get<given.a_decorated_projection_type>(key_in_request).GetAwaiter().GetResult());

    It should_call_get_one_with = () => method_caller.Verify(_ => _.Call(
        Moq.It.IsAny<ProjectionsGetOne>(),
        request_like(key_in_request, scoped_projection_id), 
        Moq.It.IsAny<CancellationToken>()));
    
    It should_only_call_it_once = () => method_caller.VerifyNoOtherCalls();
    It should_fail_because_different_keys = () => result.ShouldNotBeNull();
}

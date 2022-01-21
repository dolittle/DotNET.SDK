// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.Runtime.Projections.Contracts;
using Dolittle.SDK.Events;
using Machine.Specifications;

namespace Dolittle.SDK.Projections.Store.for_ProjectionStore.when_getting_one_projection.typed.with_projection_id;

public class and_a_projection_state_of_a_different_type_is_returned : given.all_dependencies
{
    static Key key;
    static given.a_decorated_projection_type stored_state;
    static ProjectionId another_id;
    static given.a_decorated_projection_type result;
    
    Establish context = () =>
    {
        key = "some key";
        stored_state = new given.a_decorated_projection_type
        {
            Value = 42
        };
        another_id = "867C51C8-89AA-4003-B4BA-9182FCA894E5";
        with_projection_types(typeof(given.a_decorated_projection_type));
        get_one_returns(key, new given.a_decorated_projection_type_with_scope(){Value = 42}, ProjectionCurrentStateType.Persisted);
    };

    Because of = () => result = projection_store.Get<given.a_decorated_projection_type>(key).GetAwaiter().GetResult();

    It should_call_get_one_with = () => method_caller.Verify(_ => _.Call(
        Moq.It.IsAny<ProjectionsGetOne>(),
        request_like(key, new ScopedProjectionId(another_id, ScopeId.Default)), 
        Moq.It.IsAny<CancellationToken>()));
    
    It should_only_call_it_once = () => method_caller.VerifyNoOtherCalls();
    It should_get_back_the_expected_state = () => result.Value.ShouldEqual(stored_state.Value);
}

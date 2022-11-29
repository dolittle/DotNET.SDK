// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.Runtime.Projections.Contracts;
using Dolittle.SDK.Events;
using Machine.Specifications;

namespace Dolittle.SDK.Projections.Store.for_ProjectionStore.when_getting_one_projection.typed.with_projection_id.and_scope;

public class and_a_projection_state_with_lower_case_property_is_returned : given.all_dependencies
{
    static Key key;
    static given.a_decorated_projection_type_with_lower_case_property stored_state;
    static ProjectionId another_id;
    static ScopeId another_scope;
    static given.a_decorated_projection_type result;
    
    Establish context = () =>
    {
        key = "some key";
        stored_state = new given.a_decorated_projection_type_with_lower_case_property
        {
            value = 42
        };
        another_id = "57301A06-9FE0-41A8-B995-47EDA141C2B9";
        another_scope = "B5F1FB43-D8FF-4D01-BD32-F1E3275DCE29";
        get_one_returns(key, stored_state, ProjectionCurrentStateType.Persisted);
    };

    Because of = () => result = projection_store.Get<given.a_decorated_projection_type>(key, another_id, another_scope).GetAwaiter().GetResult();

    It should_call_get_one_with = () => method_caller.Verify(_ => _.Call(
        Moq.It.IsAny<ProjectionsGetOne>(),
        request_like(key, another_id, another_scope),
        Moq.It.IsAny<CancellationToken>()));
    
    It should_only_call_it_once = () => method_caller.VerifyNoOtherCalls();
    It should_get_back_the_expected_state = () => result.Value.ShouldEqual(stored_state.value);
}

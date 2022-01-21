// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Runtime.Projections.Contracts;
using Dolittle.SDK.Events;
using Machine.Specifications;

namespace Dolittle.SDK.Projections.Store.for_ProjectionStore.when_getting_one_projection.typed.with_projection_id.and_scope;

public class and_no_projection_state_with_key_is_stored : given.all_dependencies
{
    static Key key_in_request;
    static given.a_decorated_projection_type stored_state;
    static ProjectionId another_id;
    static ScopeId another_scope;
    static Exception result;
    
    Establish context = () =>
    {
        key_in_request = "some key";
        stored_state = new given.a_decorated_projection_type
        {
            Value = 42
        };
        another_id = "EA57D65A-3CFA-4D6B-A91C-0AFFDAA40A01";
        another_scope = "B5F1FB43-D8FF-4D01-BD32-F1E3275DCE29";
        get_one_returns("another key", stored_state, ProjectionCurrentStateType.Persisted);
    };

    Because of = () => result = Catch.Exception(() => projection_store.Get<given.a_decorated_projection_type>(key_in_request, another_id, another_scope).GetAwaiter().GetResult());

    It should_call_get_one_with = () => method_caller.Verify(_ => _.Call(
        Moq.It.IsAny<ProjectionsGetOne>(),
        request_like(key_in_request, new ScopedProjectionId(another_id, another_scope)), 
        Moq.It.IsAny<CancellationToken>()));
    
    It should_only_call_it_once = () => method_caller.VerifyNoOtherCalls();
    It should_fail = () => result.ShouldNotBeNull();
}

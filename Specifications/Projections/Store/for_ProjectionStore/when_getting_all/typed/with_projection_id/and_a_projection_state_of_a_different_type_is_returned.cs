// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.Runtime.Projections.Contracts;
using Dolittle.SDK.Events;
using Machine.Specifications;

namespace Dolittle.SDK.Projections.Store.for_ProjectionStore.when_getting_all.typed.with_projection_id;

public class and_a_projection_state_of_a_different_type_is_returned : given.all_dependencies
{
    static Key key;
    static ProjectionId another_id;
    static given.a_decorated_projection_type result;
    
    Establish context = () =>
    {
        key = "some key";
        another_id = "867C51C8-89AA-4003-B4BA-9182FCA894E5";
        get_one_returns(key, new given.a_different_projection_type{AnotherField = 42, SomeOtherValue = 43}, ProjectionCurrentStateType.Persisted);
    };

    Because of = () => result = projection_store.Get<given.a_decorated_projection_type>(key, another_id).GetAwaiter().GetResult();

    It should_call_get_one_with = () => method_caller.Verify(_ => _.Call(
        Moq.It.IsAny<ProjectionsGetOne>(),
        request_like(key, new ScopedProjectionId(another_id, ScopeId.Default)), 
        Moq.It.IsAny<CancellationToken>()));
    
    It should_only_call_it_once = () => method_caller.VerifyNoOtherCalls();
    It should_get_back_the_default_state = () => result.Value.ShouldEqual(default);
}

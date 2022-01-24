// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.Runtime.Projections.Contracts;
using Machine.Specifications;

namespace Dolittle.SDK.Projections.Store.for_ProjectionStore.when_getting_one_projection.typed;

public class and_a_projection_state_of_a_different_type_is_returned : given.all_dependencies
{
    static Key key;
    static ScopedProjectionId scoped_projection_id;
    static given.a_decorated_projection_type result;
    
    Establish context = () =>
    {
        key = "some key";
        with_projection_types(typeof(given.a_decorated_projection_type));
        get_one_returns(key, new given.a_different_projection_type(){SomeOtherValue = 42, AnotherField = 43}, ProjectionCurrentStateType.Persisted);
        scoped_projection_id = read_model_types.GetFor<given.a_decorated_projection_type>();
    };

    Because of = () => result = projection_store.Get<given.a_decorated_projection_type>(key).GetAwaiter().GetResult();

    It should_call_get_one_with = () => method_caller.Verify(_ => _.Call(
        Moq.It.IsAny<ProjectionsGetOne>(),
        request_like(key, scoped_projection_id), 
        Moq.It.IsAny<CancellationToken>()));
    
    It should_only_call_it_once = () => method_caller.VerifyNoOtherCalls();
    It should_get_back_the_default_state = () => result.Value.ShouldEqual(default);
}

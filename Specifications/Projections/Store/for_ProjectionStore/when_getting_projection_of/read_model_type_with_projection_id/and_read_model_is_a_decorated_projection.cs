// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;
using Machine.Specifications;

namespace Dolittle.SDK.Projections.Store.for_ProjectionStore.when_getting_projection_of.read_model_type_with_projection_id;

public class and_read_model_is_a_decorated_projection : given.all_dependencies
{
    static IProjectionOf<given.a_decorated_projection_type> result;
    
    Because of = () => result = projection_store.Of<given.a_decorated_projection_type>("345EB8B8-FAA8-4D2B-9F0D-E937524F0D88");

    It should_get_the_projection_of_a_decorated_projection_type = () => result.ShouldNotBeNull();
    It should_have_the_correct_identifier = () => result.Identifier.ShouldEqual(new ScopedProjectionId("345EB8B8-FAA8-4D2B-9F0D-E937524F0D88", ScopeId.Default));
}
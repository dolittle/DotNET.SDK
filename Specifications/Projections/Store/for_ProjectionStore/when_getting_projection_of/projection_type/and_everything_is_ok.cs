// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.SDK.Projections.Store.for_ProjectionStore.when_getting_projection_of.projection_type;

public class and_everything_is_ok : given.all_dependencies
{
    static IProjectionOf<given.a_decorated_projection_type> result;

    Establish context = () => with_projection_types(typeof(given.a_decorated_projection_type));

    Because of = () => result = projection_store.Of<given.a_decorated_projection_type>();

    It should_get_the_projection_of_a_decorated_projection_type = () => result.ShouldNotBeNull();

    It should_have_the_identifier_from_The_attribute = () => result.Identifier.ShouldEqual(read_model_types.GetFor<given.a_decorated_projection_type>().Id);
    It should_have_the_scope_from_the_attribute = () => result.Scope.ShouldEqual(read_model_types.GetFor<given.a_decorated_projection_type>().Scope);
}
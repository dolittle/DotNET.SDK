// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.SDK.Projections.Store.for_ProjectionStore.when_getting_projection_of.read_model_type_with_projection_id.in_scope;

public class and_read_model_is_a_decorated_projection : given.all_dependencies
{
    static IProjectionOf<given.a_decorated_projection_type> result;
    
    Because of = () => result = projection_store.Of<given.a_decorated_projection_type>("345EB8B8-FAA8-4D2B-9F0D-E937524F0D88", "A01DDB79-DBE9-4AD8-8ADF-88CC08A0E2FA");

    It should_get_the_projection_of_a_decorated_projection_type = () => result.ShouldNotBeNull();
    It should_override_the_identifier_from_the_attribute_with_the_provided_identifier = () => result.Identifier.ShouldEqual("345EB8B8-FAA8-4D2B-9F0D-E937524F0D88");
    It should_override_the_scope_from_the_attribute_with_the_provided_scope = () => result.Scope.ShouldEqual("A01DDB79-DBE9-4AD8-8ADF-88CC08A0E2FA");
}
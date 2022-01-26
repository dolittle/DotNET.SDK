// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;
using Machine.Specifications;

namespace Dolittle.SDK.Projections.Store.for_ProjectionStore.when_getting_projection_of.read_model_type_with_projection_id.in_scope;

public class and_everything_is_ok : given.all_dependencies
{
    static IProjectionOf<given.an_undecorated_projection_type> result;
    
    Because of = () => result = projection_store.Of<given.an_undecorated_projection_type>("21093C97-6EFC-4A4C-8B4E-28E108930F70", "51F41F57-271F-4B86-AD26-EB25FA5D921A");

    It should_get_the_projection_of_a_decorated_projection_type = () => result.ShouldNotBeNull();
    It should_have_the_correct_identifier = () => result.Identifier.ShouldEqual(new ScopedProjectionId("21093C97-6EFC-4A4C-8B4E-28E108930F70", "51F41F57-271F-4B86-AD26-EB25FA5D921A"));
}
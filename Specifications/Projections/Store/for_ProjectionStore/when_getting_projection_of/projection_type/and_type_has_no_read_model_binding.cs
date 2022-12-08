// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Common;
using Machine.Specifications;

namespace Dolittle.SDK.Projections.Store.for_ProjectionStore.when_getting_projection_of.projection_type;

public class and_type_has_no_read_model_binding : given.all_dependencies
{
    static Exception result;

    Because of = () => result = Catch.Exception(() =>  projection_store.Of<given.a_decorated_projection_type>());

    It should_fail_because_no_projection_read_model_binding_exists = () => result.ShouldBeOfExactType<MissingUniqueBindingForValue<ProjectionModelId, Type>>();
}
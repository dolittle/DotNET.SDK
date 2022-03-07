// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.SDK.Projections.Copies.for_PropertyPathResolver.when_resolving_from_expression;

public class targetting_property : given.all_dependencies
{
    Because of = () => result = resolve_from_expression(_ => _.Property);

    It should_return_correct_path = () => result.Value.ShouldEqual(nameof(given.a_type.Property));
}
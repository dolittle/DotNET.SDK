// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.SDK.Projections.Copies.MongoDB.for_ConversionsResolver.when_resolving.projection_type;

public class without_fields : given.a_resolver
{
    Because of = () => succeeded = resolver.TryGetFrom<given.projection_type_without_fields>(build_results, out conversions_result);

    It should_succeed = () => succeeded.ShouldBeTrue();
    It should_resolve_conversions = () => conversions_result.ShouldNotBeNull();
    It should_have_no_conversions = () => conversions_result.ShouldBeEmpty();
}
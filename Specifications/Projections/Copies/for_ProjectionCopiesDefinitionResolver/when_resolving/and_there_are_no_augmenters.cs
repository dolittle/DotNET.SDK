// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using SharpCompress.Archives;

namespace Dolittle.SDK.Projections.Copies.for_ProjectionCopiesDefinitionResolver.when_resolving;

public class and_there_are_no_augmenters : given.all_dependencies
{
    Because of = () => succeeded = resolver.TryResolveFor<given.projection_type>(build_results, out copies_result);

    It should_succeed = () => succeeded.ShouldBeTrue();
    It should_output_projection_copies = () => copies_result.ShouldNotBeNull();
    It should_not_result_in_failed_build_results = () => build_results.Failed.ShouldBeFalse();
}
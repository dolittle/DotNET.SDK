// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Events;
using Dolittle.SDK.Projections.Copies;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.Projections.Builder.for_ProjectionCreator.when_creating;

public class and_copies_cannot_be_resolved : given.all_dependencies
{
    Establish context = () =>
    {
        ProjectionCopies projectionCopies = null;
        projection_copies_resolver
            .Setup(_ => _.BuildFrom<given.projection_type>(Moq.It.IsAny<IClientBuildResults>(), out projectionCopies))
            .Returns(false);
    };

    Because of = () => succeeded = creator.TryCreate(identifier, scope, on_methods, build_results, out projection_result);

    It should_try_resolve_copies_once = () => projection_copies_resolver.Verify(_ => _.BuildFrom<given.projection_type>(build_results, out Moq.It.Ref<ProjectionCopies>.IsAny), Times.Once);
    It should_not_succeed = () => succeeded.ShouldBeFalse();
    It should_not_output_a_projection = () => projection_result.ShouldBeNull();
    It should_result_in_failed_build_results = () => build_results.Failed.ShouldBeTrue();
}
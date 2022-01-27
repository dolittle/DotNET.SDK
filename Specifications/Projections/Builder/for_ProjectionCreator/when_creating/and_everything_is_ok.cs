// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Events;
using Dolittle.SDK.Projections.Copies;
using Dolittle.SDK.Projections.Copies.MongoDB;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.Projections.Builder.for_ProjectionCreator.when_creating;

public class and_everything_is_ok : given.all_dependencies
{
    static ProjectionCopies projection_copies;
    Establish context = () =>
    {
        projection_copies = new ProjectionCopies(new ProjectionCopyToMongoDB("SomeName", new Dictionary<string, Runtime.Events.Processing.Contracts.ProjectionCopyToMongoDB.Types.BSONType>()));
        projection_copies_resolver
            .Setup(_ => _.TryResolveFor<given.projection_type>(Moq.It.IsAny<IClientBuildResults>(), out projection_copies))
            .Returns(true);
    };

    Because of = () => succeeded = creator.TryCreate(identifier, scope, on_methods, build_results, out projection_result);

    It should_try_resolve_copies_once = () => projection_copies_resolver.Verify(_ => _.TryResolveFor<given.projection_type>(build_results, out Moq.It.Ref<ProjectionCopies>.IsAny), Times.Once);
    It should_succeed = () => succeeded.ShouldBeTrue();
    It should_output_a_projection = () => projection_result.ShouldNotBeNull();
    It should_have_the_correct_identifier = () => projection_result.Identifier.ShouldEqual(identifier);
    It should_have_the_correct_scope = () => projection_result.ScopeId.ShouldEqual(scope);
    It should_have_the_correct_copies = () => projection_result.Copies.ShouldEqual(projection_copies);
    It should_have_the_correct_read_model_type = () => projection_result.ProjectionType.ShouldEqual(typeof(given.projection_type));
}
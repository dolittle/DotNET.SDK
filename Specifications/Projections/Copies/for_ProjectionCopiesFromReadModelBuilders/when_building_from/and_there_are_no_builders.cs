// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Projections.Builder.Copies;
using Dolittle.SDK.Projections.Copies.for_ProjectionCopiesFromReadModelBuilders.given;
using Dolittle.SDK.Projections.Copies.MongoDB;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.Projections.Copies.for_ProjectionCopiesFromReadModelBuilders.when_building_from;

public class and_there_are_no_augmenters : all_dependencies
{
    static Mock<IProjectionCopyDefinitionBuilder<projection_type>> builder;

    Establish context = () =>
    {
        builder = new Mock<IProjectionCopyDefinitionBuilder<projection_type>>();
        builder
            .Setup(_ => _.TryBuild(Moq.It.IsAny<IClientBuildResults>(), out copies_result))
            .Callback(new try_build_callback((IClientBuildResults _, out ProjectionCopies copies) => copies = new ProjectionCopies(null)))
            .Returns(true);
        use_definition_builder(builder.Object);
    };
    
    Because of = () => succeeded = FromReadModelBuilder.TryBuildFrom<projection_type>(build_results, out copies_result);

    It should_succeed = () => succeeded.ShouldBeTrue();
    It should_output_projection_copies = () => copies_result.ShouldNotBeNull();
    It should_not_result_in_failed_build_results = () => build_results.Failed.ShouldBeFalse();
}
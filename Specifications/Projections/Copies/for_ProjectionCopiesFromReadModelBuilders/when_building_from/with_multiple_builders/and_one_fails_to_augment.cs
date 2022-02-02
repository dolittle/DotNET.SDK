// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Projections.Builder.Copies;
using Dolittle.SDK.Projections.Copies.for_ProjectionCopiesFromReadModelBuilders.given;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.Projections.Copies.for_ProjectionCopiesFromReadModelBuilders.when_building_from.with_multiple_builders;

public class and_one_fails_to_augment : all_dependencies
{
    static Mock<IProjectionCopyDefinitionBuilder<projection_type>> builder;
    static Mock<ICanBuildCopyDefinitionFromReadModel> succeeding_augmenter;
    static Mock<ICanBuildCopyDefinitionFromReadModel> failing_augmenter;
    Establish context = () =>
    {
        builder = new Mock<IProjectionCopyDefinitionBuilder<projection_type>>();
        builder
            .Setup(_ => _.TryBuild(Moq.It.IsAny<IClientBuildResults>(), out copies_result))
            .Returns(true);
        use_definition_builder(builder.Object);
        succeeding_augmenter = new Mock<ICanBuildCopyDefinitionFromReadModel>();
        succeeding_augmenter
            .Setup(_ => _.BuildFrom(Moq.It.IsAny<IClientBuildResults>(), Moq.It.IsAny<IProjectionCopyDefinitionBuilder<projection_type>>()))
            .Returns(true);
        succeeding_augmenter
            .Setup(_ => _.CanBuildFrom<projection_type>())
            .Returns(true);
        failing_augmenter = new Mock<ICanBuildCopyDefinitionFromReadModel>();
        failing_augmenter
            .Setup(_ => _.BuildFrom(Moq.It.IsAny<IClientBuildResults>(), Moq.It.IsAny<IProjectionCopyDefinitionBuilder<projection_type>>()))
            .Returns(false);
        failing_augmenter
            .Setup(_ => _.CanBuildFrom<projection_type>())
            .Returns(true);;
        add_augmenter(succeeding_augmenter, failing_augmenter);
    };
    
    Because of = () => succeeded = FromReadModelBuilder.TryBuildFrom<projection_type>(build_results, out copies_result);

    It should_call_succeeding_augmenter_once = () => succeeding_augmenter.Verify(_ => _.BuildFrom(build_results,  builder.Object), Times.Once);
    It should_call_failing_augmenter_once = () => failing_augmenter.Verify(_ => _.BuildFrom(build_results,  builder.Object), Times.Once);
    It should_not_succeed = () => succeeded.ShouldBeFalse();
    It should_not_output_projection_copies = () => copies_result.ShouldBeNull();
    It should_result_in_failed_build_results = () => build_results.Failed.ShouldBeTrue();
}
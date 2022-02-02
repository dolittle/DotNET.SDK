// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Projections.Builder.Copies;
using Dolittle.SDK.Projections.Copies.for_ProjectionCopiesFromReadModelBuilders.given;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.Projections.Copies.for_ProjectionCopiesFromReadModelBuilders.when_building_from.with_multiple_builders;

public class and_everything_is_ok : all_dependencies
{
    static Mock<IProjectionCopyDefinitionBuilder<projection_type>> builder;
    static Mock<ICanBuildCopyDefinitionFromReadModel> first_augmenter;
    static Mock<ICanBuildCopyDefinitionFromReadModel> second_augmenter;
    
    Establish context = () =>
    {
        builder = new Mock<IProjectionCopyDefinitionBuilder<projection_type>>();
        builder
            .Setup(_ => _.TryBuild(Moq.It.IsAny<IClientBuildResults>(), out copies_result))
            .Callback(new try_build_callback((IClientBuildResults _, out ProjectionCopies copies) => copies = new ProjectionCopies(null)))
            .Returns(true);
        use_definition_builder(builder.Object);
        copies_result = new ProjectionCopies(null);
        first_augmenter = new Mock<ICanBuildCopyDefinitionFromReadModel>();
        first_augmenter
            .Setup(_ => _.BuildFrom(Moq.It.IsAny<IClientBuildResults>(), Moq.It.IsAny<IProjectionCopyDefinitionBuilder<projection_type>>()))
            .Returns(true);
        first_augmenter
            .Setup(_ => _.CanBuildFrom<projection_type>())
            .Returns(true);
        second_augmenter = new Mock<ICanBuildCopyDefinitionFromReadModel>();
        second_augmenter
            .Setup(_ => _.BuildFrom(Moq.It.IsAny<IClientBuildResults>(), Moq.It.IsAny<IProjectionCopyDefinitionBuilder<projection_type>>()))
            .Returns(true);
        second_augmenter
            .Setup(_ => _.CanBuildFrom<projection_type>())
            .Returns(true);
        add_augmenter(first_augmenter, second_augmenter);
    };
    
    Because of = () => succeeded = FromReadModelBuilder.TryBuildFrom<projection_type>(build_results, out copies_result);

    It should_call_first_augmenter_once = () => first_augmenter.Verify(_ => _.BuildFrom(build_results, builder.Object), Times.Once);
    It should_call_second_augmenter_once = () => second_augmenter.Verify(_ => _.BuildFrom(build_results,  builder.Object), Times.Once);
    It should_succeed = () => succeeded.ShouldBeTrue();
    It should_output_projection_copies = () => copies_result.ShouldNotBeNull();
    It should_not_result_in_failed_build_results = () => build_results.Failed.ShouldBeFalse();
}
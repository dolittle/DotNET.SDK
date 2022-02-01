// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Common.ClientSetup;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.Projections.Copies.for_ProjectionCopiesDefinitionResolver.when_resolving.with_multiple_augmenters;

public class and_one_fails_to_augment : given.all_dependencies
{
    static Mock<ICanBuildCopyDefinitionFromReadModel> succeeding_augmenter;
    static Mock<ICanBuildCopyDefinitionFromReadModel> failing_augmenter;
    Establish context = () =>
    {
        succeeding_augmenter = new Mock<ICanBuildCopyDefinitionFromReadModel>();
        succeeding_augmenter
            .Setup(_ => _.BuildFrom<given.projection_type>(Moq.It.IsAny<IClientBuildResults>(), Moq.It.IsAny<ProjectionCopies>(), out Moq.It.Ref<ProjectionCopies>.IsAny))
            .Returns(true);
        succeeding_augmenter
            .Setup(_ => _.CanBuildFrom<given.projection_type>())
            .Returns(true);
        failing_augmenter = new Mock<ICanBuildCopyDefinitionFromReadModel>();
        failing_augmenter
            .Setup(_ => _.BuildFrom<given.projection_type>(Moq.It.IsAny<IClientBuildResults>(), Moq.It.IsAny<ProjectionCopies>(), out Moq.It.Ref<ProjectionCopies>.IsAny))
            .Returns(false);
        failing_augmenter
            .Setup(_ => _.CanBuildFrom<given.projection_type>())
            .Returns(true);;
        add_augmenter(succeeding_augmenter, failing_augmenter);
    };
    
    Because of = () => succeeded = FromReadModelBuilder.TryResolveFor<given.projection_type>(build_results, out copies_result);

    It should_call_succeeding_augmenter_once = () => succeeding_augmenter.Verify(_ => _.BuildFrom<given.projection_type>(build_results, Moq.It.IsAny<ProjectionCopies>(), out Moq.It.Ref<ProjectionCopies>.IsAny), Times.Once);
    It should_call_failing_augmenter_once = () => failing_augmenter.Verify(_ => _.BuildFrom<given.projection_type>(build_results, Moq.It.IsAny<ProjectionCopies>(), out Moq.It.Ref<ProjectionCopies>.IsAny), Times.Once);
    It should_not_succeed = () => succeeded.ShouldBeFalse();
    It should_not_output_projection_copies = () => copies_result.ShouldBeNull();
    It should_result_in_failed_build_results = () => build_results.Failed.ShouldBeTrue();
}
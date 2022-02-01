// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Common.ClientSetup;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.Projections.Copies.for_ProjectionCopiesDefinitionResolver.when_resolving.with_multiple_augmenters;

public class and_none_can_augment : given.all_dependencies
{
    static Mock<ICanBuildCopyDefinitionFromReadModel> first_augmenter;
    static Mock<ICanBuildCopyDefinitionFromReadModel> second_augmenter;
    
    Establish context = () =>
    {
        first_augmenter = new Mock<ICanBuildCopyDefinitionFromReadModel>();
        first_augmenter
            .Setup(_ => _.CanBuildFrom<given.projection_type>())
            .Returns(false);
        second_augmenter = new Mock<ICanBuildCopyDefinitionFromReadModel>();
        second_augmenter
            .Setup(_ => _.CanBuildFrom<given.projection_type>())
            .Returns(false);
        add_augmenter(first_augmenter, second_augmenter);
    };
    
    Because of = () => succeeded = FromReadModelBuilder.TryResolveFor<given.projection_type>(build_results, out copies_result);

    It should_not_call_first_augmenter = () => first_augmenter.Verify(_ => _.BuildFrom<given.projection_type>(build_results, Moq.It.IsAny<ProjectionCopies>(), out Moq.It.Ref<ProjectionCopies>.IsAny), Times.Never);
    It should_not_call_second_augmenter = () => second_augmenter.Verify(_ => _.BuildFrom<given.projection_type>(build_results, Moq.It.IsAny<ProjectionCopies>(), out Moq.It.Ref<ProjectionCopies>.IsAny), Times.Never);
    It should_succeed = () => succeeded.ShouldBeTrue();
    It should_output_projection_copies = () => copies_result.ShouldNotBeNull();
    It should_not_result_in_failed_build_results = () => build_results.Failed.ShouldBeFalse();
}
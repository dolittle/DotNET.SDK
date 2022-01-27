// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Common.ClientSetup;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.Projections.Copies.for_ProjectionCopiesDefinitionResolver.when_resolving.with_multiple_augmenters;

public class and_everything_is_ok : given.all_dependencies
{
    static Mock<ICanAugmentProjectionCopy> first_augmenter;
    static Mock<ICanAugmentProjectionCopy> second_augmenter;
    
    Establish context = () =>
    {
        copies_result = new ProjectionCopies(null);
        first_augmenter = new Mock<ICanAugmentProjectionCopy>();
        first_augmenter
            .Setup(_ => _.TryAugment<given.projection_type>(Moq.It.IsAny<IClientBuildResults>(), Moq.It.IsAny<ProjectionCopies>(), out copies_result))
            .Returns(true);
        first_augmenter
            .Setup(_ => _.CanAugment<given.projection_type>())
            .Returns(true);
        second_augmenter = new Mock<ICanAugmentProjectionCopy>();
        second_augmenter
            .Setup(_ => _.TryAugment<given.projection_type>(Moq.It.IsAny<IClientBuildResults>(), Moq.It.IsAny<ProjectionCopies>(), out copies_result))
            .Returns(true);
        second_augmenter
            .Setup(_ => _.CanAugment<given.projection_type>())
            .Returns(true);
        add_augmenter(first_augmenter, second_augmenter);
    };
    
    Because of = () => succeeded = resolver.TryResolveFor<given.projection_type>(build_results, out copies_result);

    It should_call_first_augmenter_once = () => first_augmenter.Verify(_ => _.TryAugment<given.projection_type>(build_results, Moq.It.IsAny<ProjectionCopies>(), out Moq.It.Ref<ProjectionCopies>.IsAny), Times.Once);
    It should_call_second_augmenter_once = () => second_augmenter.Verify(_ => _.TryAugment<given.projection_type>(build_results, Moq.It.IsAny<ProjectionCopies>(), out Moq.It.Ref<ProjectionCopies>.IsAny), Times.Once);
    It should_succeed = () => succeeded.ShouldBeTrue();
    It should_output_projection_copies = () => copies_result.ShouldNotBeNull();
    It should_not_result_in_failed_build_results = () => build_results.Failed.ShouldBeFalse();
}
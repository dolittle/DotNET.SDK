// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Projections.Builder.Copies.for_ProjectionCopyDefinitionBuilder.given;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.Projections.Builder.Copies.for_ProjectionCopyDefinitionBuilder.when_building;

public class and_copy_to_mongo_is_not_called : all_dependencies
{
    static Mock<MongoDB.Internal.IProjectionCopyToMongoDBBuilder<projection_type>> builder;
    Establish context = () =>
    {
        builder = get_mongo_db_builder_for<projection_type>();
    };

    Because of = () => succeeded = get_definition_builder_with<projection_type>(builder.Object).TryBuild(identifier, build_results, out result_copies);

    It should_not_fail = () => succeeded.ShouldBeTrue();
    It should_output_projection_copies = () => result_copies.ShouldNotBeNull();
    It should_not_copy_to_mongo_db = () => result_copies.MongoDB.ShouldCopy.ShouldBeFalse();
}
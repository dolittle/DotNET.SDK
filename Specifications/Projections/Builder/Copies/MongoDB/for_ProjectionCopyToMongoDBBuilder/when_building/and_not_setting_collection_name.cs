// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Projections.Copies.MongoDB;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB.for_ProjectionCopyToMongoDBBuilder.when_building;

public class and_not_setting_collection_name : given.all_dependencies
{
    static ProjectionCopyToMongoDBBuilder<given.read_model_type> builder;
    static ProjectionMongoDBCopyCollectionName name_of_type;
    Establish context = () =>
    {
        builder = setup_for<given.read_model_type>();
        name_of_type = nameof(given.read_model_type);
    };
    
    Because of = () => succeeded = builder.TryBuild(build_results, out copy_definition_result);

    It should_not_fail = () => succeeded.ShouldBeTrue();
    It should_output_a_copy_definition = () => copy_definition_result.ShouldNotBeNull();
    It should_have_the_type_name_as_collection_name = () => copy_definition_result.CollectionName.ShouldEqual(name_of_type);
    It should_have_no_conversions = () => copy_definition_result.Conversions.ShouldBeEmpty();
    It should_copy_to_mongo = () => copy_definition_result.ShouldCopy.ShouldBeTrue();
    It should_validate_collection_name = () => collection_name_validator.Verify(_ => _.Validate(build_results, name_of_type), Times.Once);
    It should_get_default_conversions = () => conversions_from_read_model.Verify(_ => _.TryBuildFrom<given.read_model_type>(), Times.Once);
    It should_not_have_failed_build_results = () => build_results.Failed.ShouldBeFalse();
}
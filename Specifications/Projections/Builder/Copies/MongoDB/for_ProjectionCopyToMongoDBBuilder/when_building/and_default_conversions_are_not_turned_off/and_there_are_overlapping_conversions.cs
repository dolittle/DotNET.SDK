// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.SDK.Projections.Copies;
using Dolittle.SDK.Projections.Copies.MongoDB;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB.for_ProjectionCopyToMongoDBBuilder.when_building.and_default_conversions_are_not_turned_off;

public class and_there_are_overlapping_conversions : given.all_dependencies
{
    static ProjectionCopyToMongoDBBuilder<given.read_model_type> builder;
    static ProjectionMongoDBCopyCollectionName name_of_type;
    static ProjectionPropertyPath _overlapPropertyPath;
    static Conversion default_conversion;
    static Conversion explicit_conversion;
    
    Establish context = () =>
    {
        default_conversion = Conversion.Guid;
        explicit_conversion = Conversion.DateTime;
        _overlapPropertyPath = nameof(given.read_model_type.Field);
        builder = setup_for<given.read_model_type>();
        name_of_type = nameof(given.read_model_type);
        conversions_from_read_model
            .Setup(_ => _.TryBuildFrom<given.read_model_type>()).Returns(new Dictionary<ProjectionPropertyPath, Conversion>
            {
                {
                    _overlapPropertyPath, default_conversion
                }
            });
        builder.WithConversion(_overlapPropertyPath, explicit_conversion);
    };
    
    Because of = () => succeeded = builder.TryBuild(build_results, out copy_definition_result);

    It should_not_fail = () => succeeded.ShouldBeTrue();
    It should_output_a_copy_definition = () => copy_definition_result.ShouldNotBeNull();
    It should_have_the_type_name_as_collection_name = () => copy_definition_result.CollectionName.ShouldEqual(name_of_type);
    It should_have_only_the_explicit_conversion = () => copy_definition_result.Conversions.ShouldContainOnly(new KeyValuePair<ProjectionPropertyPath, Conversion>(_overlapPropertyPath, explicit_conversion));
    It should_copy_to_mongo = () => copy_definition_result.ShouldCopy.ShouldBeTrue();
    It should_validate_collection_name = () => collection_name_validator.Verify(_ => _.Validate(build_results, name_of_type), Times.Once);
    It shouldget_default_conversions = () => conversions_from_read_model.Verify(_ => _.TryBuildFrom<given.read_model_type>(), Times.Once);
    It should_not_have_failed_build_results = () => build_results.Failed.ShouldBeFalse();
}
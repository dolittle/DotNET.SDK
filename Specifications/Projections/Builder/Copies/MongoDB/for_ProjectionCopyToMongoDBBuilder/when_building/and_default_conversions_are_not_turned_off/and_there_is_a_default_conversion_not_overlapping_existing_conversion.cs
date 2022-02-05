// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Linq.Expressions;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Projections.Copies;
using Dolittle.SDK.Projections.Copies.MongoDB;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB.for_ProjectionCopyToMongoDBBuilder.when_building.and_default_conversions_are_not_turned_off;

public class and_there_is_a_default_conversion_not_overlapping_existing_conversion : given.all_dependencies
{
    static ProjectionCopyToMongoDBBuilder<given.read_model_type> builder;
    static ProjectionMongoDBCopyCollectionName name_of_type;
    static PropertyPath existing_property_path;
    static PropertyPath additional_property_path;
    static Conversion existing_conversion;
    static Conversion additional_conversion;
    
    Establish context = () =>
    {
        existing_conversion = Conversion.Guid;
        additional_conversion = Conversion.DateTime;
        existing_property_path = nameof(given.read_model_type.Field);
        additional_property_path = nameof(given.read_model_type.AProperty);
        builder = setup_for<given.read_model_type>();
        
        with_explicit_conversions(builder, (_ => _.Field, existing_property_path, existing_conversion));
        name_of_type = nameof(given.read_model_type);
        conversions_from_bson_class_map
            .Setup(_ => _.TryBuildFrom<given.read_model_type>(Moq.It.IsAny<IClientBuildResults>(), Moq.It.IsAny<PropertyConversions>()))
            .Callback<IClientBuildResults, PropertyConversions>((_, conversions) => conversions.AddConversion(additional_property_path, additional_conversion))
            .Returns(true);
    };
    
    Because of = () => succeeded = builder.TryBuild(build_results, out copy_definition_result);

    It should_not_fail = () => succeeded.ShouldBeTrue();
    It should_output_a_copy_definition = () => copy_definition_result.ShouldNotBeNull();
    It should_have_the_type_name_as_collection_name = () => copy_definition_result.CollectionName.ShouldEqual(name_of_type);
    It should_have_only_have_the_existing_and_additional_conversions = () => should_only_contain_conversions(
        (existing_conversion, existing_property_path),
        (additional_conversion, additional_property_path));
    It should_copy_to_mongo = () => copy_definition_result.ShouldCopy.ShouldBeTrue();
    It should_validate_collection_name = () => collection_name_validator.Verify(_ => _.Validate(build_results, name_of_type), Times.Once);
    It should_get_default_conversions = () => conversions_from_bson_class_map.Verify(_ => _.TryBuildFrom<given.read_model_type>(build_results, Moq.It.IsAny<PropertyConversions>()), Times.Once);
    It should_not_have_failed_build_results = () => build_results.Failed.ShouldBeFalse();
}
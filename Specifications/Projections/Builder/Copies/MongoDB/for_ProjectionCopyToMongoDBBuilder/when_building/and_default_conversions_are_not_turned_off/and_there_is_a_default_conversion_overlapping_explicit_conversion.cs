// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Projections.Copies;
using Dolittle.SDK.Projections.Copies.MongoDB;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB.for_ProjectionCopyToMongoDBBuilder.when_building.and_default_conversions_are_not_turned_off;

public class and_there_is_a_default_conversion_overlapping_explicit_conversion : given.all_dependencies
{
    static ProjectionCopyToMongoDBBuilder<given.read_model_type> builder;
    static MongoDBCopyCollectionName name_of_type;
    static PropertyPath overlap_property_path;
    static Conversion default_conversion;
    static Conversion explicit_conversion;
    
    Establish context = () =>
    {
        default_conversion = Conversion.Guid;
        explicit_conversion = Conversion.Date;
        overlap_property_path = nameof(given.read_model_type.Field);
        builder = setup_for<given.read_model_type>();
        
        with_explicit_conversions(builder, (_ => _.Field, overlap_property_path, explicit_conversion));
        name_of_type = nameof(given.read_model_type);
        conversions_from_bson_class_map
            .Setup(_ => _.TryBuildFrom<given.read_model_type>(Moq.It.IsAny<IClientBuildResults>(), Moq.It.IsAny<IPropertyConversions>()))
            .Callback<IClientBuildResults, IPropertyConversions>((_, conversions) => conversions.AddConversion(overlap_property_path, default_conversion))
            .Returns(true);
    };
    
    Because of = () => succeeded = builder.TryBuild(build_results, out copy_definition_result);

    It should_not_fail = () => succeeded.ShouldBeTrue();
    It should_output_a_copy_definition = () => copy_definition_result.ShouldNotBeNull();
    It should_have_the_type_name_as_collection_name = () => copy_definition_result.CollectionName.ShouldEqual(name_of_type);
    It should_have_only_the_explicit_conversion = () => should_only_contain_conversions((explicit_conversion, overlap_property_path));
    It should_copy_to_mongo = () => copy_definition_result.ShouldCopy.ShouldBeTrue();
    It should_validate_collection_name = () => collection_name_validator.Verify(_ => _.Validate(build_results, name_of_type), Times.Once);
    It should_get_default_conversions = () => conversions_from_bson_class_map.Verify(_ => _.TryBuildFrom<given.read_model_type>(build_results, Moq.It.IsAny<IPropertyConversions>()), Times.Once);
    It should_not_have_failed_build_results = () => build_results.Failed.ShouldBeFalse();
}
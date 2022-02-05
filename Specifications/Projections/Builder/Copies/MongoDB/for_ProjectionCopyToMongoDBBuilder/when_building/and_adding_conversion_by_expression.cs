using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Dolittle.SDK.Projections.Copies;
using Dolittle.SDK.Projections.Copies.MongoDB;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB.for_ProjectionCopyToMongoDBBuilder.when_building;

public class and_adding_conversion_by_expression : given.all_dependencies
{
    static ProjectionCopyToMongoDBBuilder<given.read_model_type> builder;
    static ProjectionMongoDBCopyCollectionName name_of_type;
    static PropertyPath property_path;
    static Expression<Func<given.read_model_type, int>> expression;
    static Conversion conversion;

    Establish context = () =>
    {
        builder = setup_for<given.read_model_type>();
        name_of_type = nameof(given.read_model_type);
        expression = property_path_expression_for<given.read_model_type, int>(_ => _.Field);
        property_path = nameof(given.read_model_type.Field);
        conversion = Conversion.Guid;
        with_explicit_conversions(builder, (expression, property_path, conversion));
    };

    Because of = () => succeeded = builder.TryBuild(build_results, out copy_definition_result);

    It should_not_fail = () => succeeded.ShouldBeTrue();
    It should_output_a_copy_definition = () => copy_definition_result.ShouldNotBeNull();
    It should_have_the_type_name_as_collection_name = () => copy_definition_result.CollectionName.ShouldEqual(name_of_type);
    It should_only_have_the_one_conversion = () => should_only_contain_conversions((conversion, property_path));
    It should_copy_to_mongo = () => copy_definition_result.ShouldCopy.ShouldBeTrue();
    It should_resolve_property_path_from_expression = () => property_path_resolver.Verify(_ => _.FromExpression(expression), Times.Once);
    It should_validate_collection_name = () => collection_name_validator.Verify(_ => _.Validate(build_results, name_of_type), Times.Once);
    It should_get_default_conversions = () => conversions_from_bson_class_map.Verify(_ => _.TryBuildFrom<given.read_model_type>(build_results, Moq.It.IsAny<PropertyConversions>()), Times.Once);
    It should_not_have_failed_build_results = () => build_results.Failed.ShouldBeFalse();
}
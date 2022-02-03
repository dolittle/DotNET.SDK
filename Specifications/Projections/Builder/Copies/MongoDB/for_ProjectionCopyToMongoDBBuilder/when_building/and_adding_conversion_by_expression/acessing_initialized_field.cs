using System.Collections.Generic;
using Dolittle.SDK.Projections.Copies;
using Dolittle.SDK.Projections.Copies.MongoDB;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB.for_ProjectionCopyToMongoDBBuilder.when_building.and_adding_conversion_by_expression;

public class acessing_initialized_field : given.all_dependencies
{
    static ProjectionCopyToMongoDBBuilder<given.read_model_type> builder;
    static ProjectionMongoDBCopyCollectionName name_of_type;
    static ProjectionField field;
    static Conversion conversion;
    
    Establish context = () =>
    {
        builder = setup_for<given.read_model_type>();
        name_of_type = nameof(given.read_model_type);
        field = nameof(given.read_model_type.InitializedField);
        conversion = Conversion.Guid;
        builder.WithConversion(_ => _.InitializedField, conversion);
    };
    
    Because of = () => succeeded = builder.TryBuild(build_results, out copy_definition_result);

    It should_not_fail = () => succeeded.ShouldBeTrue();
    It should_output_a_copy_definition = () => copy_definition_result.ShouldNotBeNull();
    It should_have_the_type_name_as_collection_name = () => copy_definition_result.CollectionName.ShouldEqual(name_of_type);
    It should_have_the_conversion = () => copy_definition_result.Conversions.ShouldContainOnly(new KeyValuePair<ProjectionField, Conversion>(field, conversion));
    It should_copy_to_mongo = () => copy_definition_result.ShouldCopy.ShouldBeTrue();
    It should_validate_collection_name = () => collection_name_validator.Verify(_ => _.Validate(build_results, name_of_type), Times.Once);
    It should_get_default_conversions = () => conversions_from_read_model.Verify(_ => _.GetFrom<given.read_model_type>(), Times.Once);
    It should_not_have_failed_build_results = () => build_results.Failed.ShouldBeFalse();
}
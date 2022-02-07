using System;
using Dolittle.SDK.Projections.Copies.MongoDB;
using Machine.Specifications;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB.for_ConversionsFromBsonClassMapBuilder.when_building_from_type;

public class with_complex_field_containing_type_that_needs_conversion : given.all_dependencies
{
    public class read_model
    {
        public inner_type ComplexField;
        public string Property { get; }
    }

    public class inner_type
    {
        public Guid InnerGuidField;
    }

    Because of = () => succeeded = builder.TryBuildFrom<read_model>(build_results, conversions.Object);

    It should_succeed = () => succeeded.ShouldBeTrue();
    It should_add_conversion_for_inner_field = () => conversions.Verify(_ => _.AddConversion("ComplexField.InnerGuidField", Conversion.Guid));
    It should_not_add_anything_else = () => conversions.VerifyNoOtherCalls();
}
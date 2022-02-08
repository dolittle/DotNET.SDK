using System;
using Dolittle.SDK.Projections.Copies.MongoDB;
using Machine.Specifications;
using MongoDB.Bson.Serialization;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB.for_ConversionsFromBsonClassMapAdder.when_adding_from_class_map.from_lookup_for_read_model_type;

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

    Because of = () => adder.AddFrom(BsonClassMap.LookupClassMap(typeof(read_model)), build_results, conversions.Object);

    
    It should_add_conversion_for_inner_field = () => conversions.Verify(_ => _.AddConversion("ComplexField.InnerGuidField", Conversion.GuidAsCSharpLegacyBinary));
    It should_not_add_anything_else = () => conversions.VerifyNoOtherCalls();
}
using Dolittle.SDK.Projections.Copies.MongoDB;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB.for_ConversionsFromMongoDBConvertToAttributesBuilder.when_building_from;

public class type_with_attributed_property_and_field : given.all_dependencies
{
    public class read_model_with_attibuted_property_and_field
    {
        [MongoDBConvertTo(Conversion.Date)]
        public int AField;

        [MongoDBConvertTo(Conversion.Guid)]
        public int AProperty { get; set; }

        public int AMethod() => 2;
    }
    Because of = () => builder.BuildFrom<read_model_with_attibuted_property_and_field>(build_results, conversions.Object);
    
    It should_add_the_conversion_for_property = () => conversions.Verify(_ => _.AddConversion(nameof(read_model_with_attibuted_property_and_field.AProperty), Conversion.Guid), Times.Once);
    It should_add_the_conversion_for_field = () => conversions.Verify(_ => _.AddConversion(nameof(read_model_with_attibuted_property_and_field.AField), Conversion.Date), Times.Once);
    It should_not_add_anything_else = () => conversions.VerifyNoOtherCalls();
}
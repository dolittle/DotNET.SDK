using Dolittle.SDK.Projections.Copies;
using Dolittle.SDK.Projections.Copies.MongoDB;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB.for_ConversionsFromMongoDBConvertToAttributesBuilder.when_building_from;

public class type_with_inner_attributed_type_without_attribute : given.all_dependencies
{
    public class read_model_with_inner_attributed_type_without_attribute
    {
        public int AField;
    
        public int AProperty { get; set; }

        public int AMethod() => 2;
        
        public read_model_with_an_attribute RecursiveField;
    }
    public class read_model_with_an_attribute
    {
        [MongoDBConvertTo(Conversion.Guid)]
        public int AField;

        public int AProperty { get; set; }

        public int AMethod() => 2;
    }

    Because of = () => succeeded = builder.TryBuildFrom<read_model_with_inner_attributed_type_without_attribute>(build_results, conversions.Object);

    It should_succeed = () => succeeded.ShouldBeTrue();
    It should_add_conversion_for_inner_attributed_field = () => conversions.Verify(_ => _.AddConversion("RecursiveField.AField", Conversion.Guid), Times.Once);
    It should_not_add_anything_else = () => conversions.VerifyNoOtherCalls();
}
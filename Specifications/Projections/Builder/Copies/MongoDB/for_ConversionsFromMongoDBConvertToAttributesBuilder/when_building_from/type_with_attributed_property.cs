using Dolittle.SDK.Projections.Copies.MongoDB;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB.for_ConversionsFromMongoDBConvertToAttributesBuilder.when_building_from;

public class type_with_attributed_property : given.all_dependencies
{
    public class read_model_with_attibuted_property
    {
        public int AField;

        [MongoDBConvertTo(Conversion.Guid)]
        public int AProperty { get; set; }

        public int AMethod() => 2;
    }
    Because of = () => succeeded = builder.TryBuildFrom<read_model_with_attibuted_property>(build_results, conversions.Object);

    It should_succeed = () => succeeded.ShouldBeTrue();
    It should_add_the_conversion = () => conversions.Verify(_ => _.AddConversion(nameof(read_model_with_attibuted_property.AProperty), Conversion.Guid), Times.Once);
    It should_not_add_anything_else = () => conversions.VerifyNoOtherCalls();
}
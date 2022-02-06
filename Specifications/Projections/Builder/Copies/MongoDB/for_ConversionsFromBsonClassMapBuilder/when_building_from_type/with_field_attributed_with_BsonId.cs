using Machine.Specifications;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB.for_ConversionsFromBsonClassMapBuilder.when_building_from_type;

public class with_field_attributed_with_BsonId : given.all_dependencies
{
    public class read_model
    {
        [BsonId]
        public string IdField;
        public string Field;
        public string Property { get; }
    }

    Because of = () => succeeded = builder.TryBuildFrom<read_model>(build_results, conversions.Object);

    It should_succeed = () => succeeded.ShouldBeTrue();
    It should_rename__id_to_IdField = () => conversions.Verify(_ => _.AddRenaming("_id", "IdField"));
    It should_not_add_anything_else = () => conversions.VerifyNoOtherCalls();
}
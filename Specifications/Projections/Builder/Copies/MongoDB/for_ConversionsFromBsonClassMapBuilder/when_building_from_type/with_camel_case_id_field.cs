using Machine.Specifications;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB.for_ConversionsFromBsonClassMapBuilder.when_building_from_type;

public class with_camel_case_id_field : given.all_dependencies
{
    public class read_model
    {
        public string id;
        public string Field;
        public string Property { get; }
    }

    Because of = () => succeeded = builder.TryBuildFrom<read_model>(build_results, conversions.Object);

    It should_succeed = () => succeeded.ShouldBeTrue();
    It should_rename__id_to_id = () => conversions.Verify(_ => _.AddRenaming("id", "_id"));
    It should_not_add_anything_else = () => conversions.VerifyNoOtherCalls();
}
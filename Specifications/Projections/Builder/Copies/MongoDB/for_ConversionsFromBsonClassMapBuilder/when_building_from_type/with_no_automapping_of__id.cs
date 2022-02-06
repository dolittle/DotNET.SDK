using Machine.Specifications;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB.for_ConversionsFromBsonClassMapBuilder.when_building_from_type;

public class with_no_automapping_of__id : given.all_dependencies
{
    public class read_model
    {
        public string Field;
        public string Property { get; }
    }

    Because of = () => succeeded = builder.TryBuildFrom<read_model>(build_results, conversions.Object);

    It should_succeed = () => succeeded.ShouldBeTrue();
    It should_not_add_anything = () => conversions.VerifyNoOtherCalls();
}
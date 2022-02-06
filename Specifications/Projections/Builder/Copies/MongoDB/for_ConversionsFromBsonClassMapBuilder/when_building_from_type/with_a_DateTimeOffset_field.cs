using System;
using Dolittle.SDK.Projections.Copies.MongoDB;
using Machine.Specifications;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB.for_ConversionsFromBsonClassMapBuilder.when_building_from_type;

public class with_a_DateTimeOffset_field : given.all_dependencies
{
    public class read_model
    {
        public DateTimeOffset DateTimeOffsetField;
        public string Property { get; }
    }

    Because of = () => succeeded = builder.TryBuildFrom<read_model>(build_results, conversions.Object);

    It should_succeed = () => succeeded.ShouldBeTrue();
    It should_add_conversion_to_date_time_for_date_time_offset_field = () => conversions.Verify(_ => _.AddConversion("DateTimeOffsetField", Conversion.DateTime));
    It should_not_add_anything_else = () => conversions.VerifyNoOtherCalls();
}
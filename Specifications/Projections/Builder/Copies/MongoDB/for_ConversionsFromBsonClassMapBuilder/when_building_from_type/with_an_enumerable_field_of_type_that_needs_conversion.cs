using System;
using System.Collections.Generic;
using Dolittle.SDK.Projections.Copies.MongoDB;
using Machine.Specifications;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB.for_ConversionsFromBsonClassMapBuilder.when_building_from_type;

public class with_an_enumerable_field_of_type_that_needs_conversion : given.all_dependencies
{
    public class read_model
    {
        public IEnumerable<Guid> EnumerableGuidField;
        public string Property { get; }
    }

    Because of = () => succeeded = builder.TryBuildFrom<read_model>(build_results, conversions.Object);

    It should_succeed = () => succeeded.ShouldBeTrue();
    It should_add_conversion_for_enumerable_field = () => conversions.Verify(_ => _.AddConversion("EnumerableGuidField", Conversion.Guid));
    It should_not_add_anything_else = () => conversions.VerifyNoOtherCalls();
}
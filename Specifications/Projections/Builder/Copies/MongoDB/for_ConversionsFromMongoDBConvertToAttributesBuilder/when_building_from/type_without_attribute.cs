// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Projections.Copies;
using Dolittle.SDK.Projections.Copies.MongoDB;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB.for_ConversionsFromMongoDBConvertToAttributesBuilder.when_building_from;

public class type_without_attribute : given.all_dependencies
{
    public class read_model_with_no_attributes
    {
        public int AField;

        public int AProperty { get; set; }

        public int AMethod() => 2;
    }
    Because of = () => succeeded = builder.TryBuildFrom<read_model_with_no_attributes>(build_results, conversions.Object);

    It should_succeed = () => succeeded.ShouldBeTrue();
    It should_not_add_any_conversions = () => conversions.Verify(_ => _.AddConversion(Moq.It.IsAny<PropertyPath>(), Moq.It.IsAny<Conversion>()), Times.Never);
    It should_not_add_anything_else = () => conversions.VerifyNoOtherCalls();
}
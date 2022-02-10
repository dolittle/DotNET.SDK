// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Projections.Copies.MongoDB;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB.for_ConversionsFromConvertToMongoDBAttributesBuilder.when_building_from;

public class type_with_endless_recursive_property : given.all_dependencies
{
    public class read_model_with_inner_none_attributed_type_with_attribute
    {
        public int AField;
    
        public int AProperty { get; set; }

        public int AMethod() => 2;

        [ConvertToMongoDB(Conversion.None)]
        public read_model_with_inner_none_attributed_type_with_attribute RecursiveField;
    }

    Because of = () => builder.BuildFrom<read_model_with_inner_none_attributed_type_with_attribute>(build_results, conversions.Object);

    It should_add_the_conversion = () => conversions.Verify(_ => _.AddConversion(nameof(read_model_with_inner_none_attributed_type_with_attribute.RecursiveField), Conversion.None), Times.Once);
    It should_not_add_anything_else = () => conversions.VerifyNoOtherCalls();
}
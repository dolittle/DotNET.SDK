// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Projections.Copies.MongoDB;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB.for_ConversionsFromMongoDBConvertToAttributesBuilder.when_building_from;

public class type_with_inner_attributed_type_with_attribute : given.all_dependencies
{
    public class read_model_with_inner_attributed_type_with_attribute
    {
        public int AField;
    
        public int AProperty { get; set; }

        public int AMethod() => 2;

        [MongoDBConvertTo(Conversion.None)]
        public read_model_with_an_attribute RecursiveField;
    }
    public class read_model_with_an_attribute
    {
        [MongoDBConvertTo(Conversion.GuidAsCSharpLegacyBinary)]
        public int AField;

        public int AProperty { get; set; }

        public int AMethod() => 2;
    }

    Because of = () => builder.BuildFrom<read_model_with_inner_attributed_type_with_attribute>(build_results, conversions.Object);

    It should_add_the_parent_conversion = () => conversions.Verify(_ => _.AddConversion(nameof(read_model_with_inner_attributed_type_with_attribute.RecursiveField), Conversion.None), Times.Once);
    It should_add_the_inner_conversion = () => conversions.Verify(_ => _.AddConversion(
        string.Join('.', nameof(read_model_with_inner_attributed_type_with_attribute.RecursiveField), nameof(read_model_with_inner_attributed_type_with_attribute.RecursiveField.AField)), Conversion.GuidAsCSharpLegacyBinary), Times.Once);
    It should_not_add_anything_else = () => conversions.VerifyNoOtherCalls();
}
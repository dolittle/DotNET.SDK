// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Projections.Copies.MongoDB;
using Machine.Specifications;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB.for_ConversionsFromBsonClassMapAdder.when_adding_from_class_map.from_lookup_for_read_model_type.with_a_guid_field;

public class with_csharp_legacy_binary_representation : given.all_dependencies
{
    public class read_model
    {
        [BsonGuidRepresentation(GuidRepresentation.CSharpLegacy)]
        public Guid GuidField;
        public string Property { get; }
    }

    Because of = () => adder.AddFrom(BsonClassMap.LookupClassMap(typeof(read_model)), build_results, conversions.Object);

    
    It should_add_conversion_to_guid_for_guid_field = () => conversions.Verify(_ => _.AddConversion("GuidField", Conversion.GuidAsCSharpLegacyBinary));
    It should_not_add_anything_else = () => conversions.VerifyNoOtherCalls();
}
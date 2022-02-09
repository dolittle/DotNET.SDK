// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Projections.Copies.MongoDB;
using Machine.Specifications;
using MongoDB.Bson.Serialization;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB.for_ConversionsFromBsonClassMapAdder.when_adding_from_class_map.from_lookup_for_read_model_type.with_a_date_time_field;

public class without_explicit_representation : given.all_dependencies
{
    public class read_model
    {
        public DateTime DateTimeField;
        public string Property { get; }
    }

    Because of = () => adder.AddFrom(BsonClassMap.LookupClassMap(typeof(read_model)), build_results, conversions.Object);

    
    It should_add_conversion_to_date_time_for_date_time_field = () => conversions.Verify(_ => _.AddConversion("DateTimeField", Conversion.DateAsDate));
    It should_not_add_anything_else = () => conversions.VerifyNoOtherCalls();
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.SDK.Projections.Copies.MongoDB;
using Machine.Specifications;
using MongoDB.Bson.Serialization;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB.for_ConversionsFromBsonClassMapAdder.when_adding_from_class_map.from_lookup_for_read_model_type;

public class with_an_enumerable_field_of_type_that_needs_conversion : given.all_dependencies
{
    public class read_model
    {
        public IEnumerable<Guid> EnumerableGuidField;
        public Guid[] ArrayGuidField;
        public IList<Guid> ListGuidField;
        public string Property { get; }
    }

    Because of = () => adder.AddFrom(BsonClassMap.LookupClassMap(typeof(read_model)), build_results, conversions.Object);

    
    It should_add_conversion_for_enumerable_field = () => conversions.Verify(_ => _.AddConversion("EnumerableGuidField", Conversion.GuidAsCSharpLegacyBinary));
    It should_add_conversion_for_array_field = () => conversions.Verify(_ => _.AddConversion("ArrayGuidField", Conversion.GuidAsCSharpLegacyBinary));
    It should_add_conversion_for_list_field = () => conversions.Verify(_ => _.AddConversion("ListGuidField", Conversion.GuidAsCSharpLegacyBinary));
    It should_not_add_anything_else = () => conversions.VerifyNoOtherCalls();
}
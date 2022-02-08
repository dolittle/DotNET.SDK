// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using MongoDB.Bson.Serialization;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB.for_ConversionsFromBsonClassMapAdder.when_adding_from_class_map.from_lookup_for_read_model_type;

public class with_pascal_case_id_field : given.all_dependencies
{
    public class read_model
    {
        public string Id;
        public string Field;
        public string Property { get; }
    }

    Because of = () => adder.AddFrom(BsonClassMap.LookupClassMap(typeof(read_model)), build_results, conversions.Object);

    
    It should_rename__id_to_Id = () => conversions.Verify(_ => _.AddRenaming("Id", "_id"));
    It should_not_add_anything_else = () => conversions.VerifyNoOtherCalls();
}
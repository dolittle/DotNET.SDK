using Machine.Specifications;
using MongoDB.Bson.Serialization;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB.for_ConversionsFromBsonClassMapAdder.when_adding_from_class_map.from_lookup_for_read_model_type;

public class with_camel_case_id_field : given.all_dependencies
{
    public class read_model
    {
        public string id;
        public string Field;
        public string Property { get; }
    }

    Because of = () => adder.AddFrom(BsonClassMap.LookupClassMap(typeof(read_model)), build_results, conversions.Object);

    
    It should_rename__id_to_id = () => conversions.Verify(_ => _.AddRenaming("id", "_id"));
    It should_not_add_anything_else = () => conversions.VerifyNoOtherCalls();
}
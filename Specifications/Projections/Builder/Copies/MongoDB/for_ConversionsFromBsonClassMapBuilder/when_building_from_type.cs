// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using MongoDB.Bson.Serialization;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB.for_ConversionsFromBsonClassMapBuilder;

public class when_building_from_type : given.all_dependencies
{
    public class read_model
    {
        public string id;
        public string Field;
        public string Property { get; }
    }

    Because of = () => builder.BuildFrom<read_model>(build_results, conversions.Object);

    It should_add_conversions_from_class_map = () => conversions_from_class_map_adder.Verify(_ => _.AddFrom(BsonClassMap.LookupClassMap(typeof(read_model)), build_results, conversions.Object), Times.Once);
}
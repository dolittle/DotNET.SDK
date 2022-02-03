// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Projections.Builder.Copies.MongoDB.for_MongoDbCopyDefinitionFromReadModelBuilder.given;
using Dolittle.SDK.Projections.Copies.MongoDB;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB.for_MongoDbCopyDefinitionFromReadModelBuilder.when_building_from.and_read_model_type;

public class is_missing_copy_to_mongo_decorator : given.all_dependencies
{
    static ProjectionMongoDBCopyCollectionName collection_name;
    static Mock<IProjectionCopyDefinitionBuilder<projection_type_without_mongo_db_copy>> definition_builder;
    static Mock<IProjectionCopyToMongoDBBuilder<projection_type_without_mongo_db_copy>> mongo_builder;
    Establish context = () =>
    {
        collection_name = "some_collection";
        mongo_builder = new Mock<IProjectionCopyToMongoDBBuilder<projection_type_without_mongo_db_copy>>();
        definition_builder = get_builder_for(mongo_builder.Object);
    };
    
    Because of = () => succeeded = CopyDefinitionFromReadModelBuilder.BuildFrom(build_results, definition_builder.Object);

    It should_not_succeed = () => succeeded.ShouldBeFalse();
    It should_result_in_failed_build_results = () => build_results.Failed.ShouldBeTrue();
    It should_not_call_copy_to_mongodb = () => definition_builder.Verify(_ => _.CopyToMongoDB(Moq.It.IsAny<Action<IProjectionCopyToMongoDBBuilder<projection_type_without_mongo_db_copy>>>()), Times.Never);
}
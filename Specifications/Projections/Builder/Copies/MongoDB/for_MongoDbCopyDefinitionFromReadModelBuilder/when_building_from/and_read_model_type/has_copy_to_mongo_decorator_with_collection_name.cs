// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Projections.Builder.Copies.MongoDB.for_MongoDbCopyDefinitionFromReadModelBuilder.given;
using Dolittle.SDK.Projections.Copies;
using Dolittle.SDK.Projections.Copies.MongoDB;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB.for_MongoDbCopyDefinitionFromReadModelBuilder.when_building_from.and_read_model_type;

public class has_copy_to_mongo_decorator_with_collection_name : given.all_dependencies
{
    static ProjectionMongoDBCopyCollectionName collection_name;
    static Mock<IProjectionCopyDefinitionBuilder<projection_type_with_mongo_db_copy_and_collection_name>> definition_builder;
    static Mock<IProjectionCopyToMongoDBBuilder<projection_type_with_mongo_db_copy_and_collection_name>> mongo_builder;
    Establish context = () =>
    {
        collection_name = "some_collection";
        mongo_builder = new Mock<IProjectionCopyToMongoDBBuilder<projection_type_with_mongo_db_copy_and_collection_name>>();
        definition_builder = get_builder_for(mongo_builder.Object);
    };
    
    Because of = () => succeeded = CopyDefinitionFromReadModelBuilder.BuildFrom(build_results, definition_builder.Object);

    It should_succeed = () => succeeded.ShouldBeTrue();
    It should_not_result_in_failed_build_results = () => build_results.Failed.ShouldBeFalse();
    It should_call_copy_to_mongodb_once = () => definition_builder.Verify(_ => _.CopyToMongoDB(Moq.It.IsAny<Action<IProjectionCopyToMongoDBBuilder<projection_type_with_mongo_db_copy_and_collection_name>>>()), Times.Once);
    It should_add_the_collection_name = () => mongo_builder.Verify(_ => _.ToCollection(collection_name), Times.Once);
    It should_not_add_any_conversions_ = () => mongo_builder.Verify(_ => _.WithConversion(Moq.It.IsAny<ProjectionPropertyPath>(), Moq.It.IsAny<Conversion>()), Times.Never);
}
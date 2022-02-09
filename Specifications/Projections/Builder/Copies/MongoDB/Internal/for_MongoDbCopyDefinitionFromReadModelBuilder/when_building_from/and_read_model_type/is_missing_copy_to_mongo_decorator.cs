// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Projections.Builder.Copies.MongoDB.Internal.for_MongoDbCopyDefinitionFromReadModelBuilder.given;
using Dolittle.SDK.Projections.Copies.MongoDB;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB.Internal.for_MongoDbCopyDefinitionFromReadModelBuilder.when_building_from.and_read_model_type;

public class is_missing_copy_to_mongo_decorator : given.all_dependencies
{
    static MongoDBCopyCollectionName collection_name;
    static Mock<IProjectionCopyToMongoDBBuilder<projection_type_without_mongo_db_copy>> mongo_builder;
    Establish context = () =>
    {
        collection_name = "some_collection";
        mongo_builder = new Mock<IProjectionCopyToMongoDBBuilder<projection_type_without_mongo_db_copy>>();
        conversions_from_convert_to_attributes
            .Setup(_ => _.BuildFrom<projection_type_without_mongo_db_copy>(Moq.It.IsAny<IClientBuildResults>(), Moq.It.IsAny<IPropertyConversions>()));
    };
    
    Because of = () => succeeded = builder.TryBuild(build_results, mongo_builder.Object);

    It should_not_succeed = () => succeeded.ShouldBeFalse();
    It should_result_in_failed_build_results = () => build_results.Failed.ShouldBeTrue();
    It should_not_build_conversions_from_attributes = () => conversions_from_convert_to_attributes.Verify(_ => _.BuildFrom<projection_type_without_mongo_db_copy>(build_results, Moq.It.IsAny<IPropertyConversions>()), Times.Never);
}
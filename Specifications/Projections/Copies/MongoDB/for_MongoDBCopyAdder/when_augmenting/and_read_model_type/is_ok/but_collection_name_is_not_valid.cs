// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Projections.Copies.MongoDB.for_MongoDBCopyAdder.given;
using Machine.Specifications;
using MongoDB.Bson;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.Projections.Copies.MongoDB.for_MongoDBCopyAdder.when_augmenting.and_read_model_type.is_ok;

public class but_collection_name_is_not_valid : given.all_dependencies
{
    static IDictionary<string, BsonType> conversions;
    static ProjectionMongoDBCopyCollectionName collection_name;
    Establish context = () =>
    {
        conversions = new Dictionary<string, BsonType>();
        collection_name = nameof(projection_type_with_mongo_db_copy);
        conversions_resolver
            .Setup(_ => _.TryResolve<projection_type_with_mongo_db_copy>(Moq.It.IsAny<IClientBuildResults>(), out conversions))
            .Returns(true);
        collection_name_validator
            .Setup(_ => _.Validate(Moq.It.IsAny<IClientBuildResults>(), Moq.It.IsAny<ProjectionMongoDBCopyCollectionName>()))
            .Returns(false);
    };
    Because of = () => succeeded = copy_adder.TryAugment<projection_type_with_mongo_db_copy>(build_results, projection_copies, out augmented_result);

    It should_not_succeed = () => succeeded.ShouldBeFalse();
    It should_not_output_augmented_result = () => augmented_result.ShouldBeNull();
    It should_validate_collection_name = () => collection_name_validator.Verify(_ => _.Validate(build_results, collection_name), Times.Once);
    It should_result_in_failed_build_results = () => build_results.Failed.ShouldBeTrue();
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Projections.Copies.MongoDB.for_MongoDBCopyAdder.given;
using Machine.Specifications;
using MongoDB.Bson;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.Projections.Copies.MongoDB.for_MongoDBCopyAdder.when_augmenting.and_read_model_type;

public class has_copy_to_mongo_decorator_with_collection_name : given.all_dependencies
{
    static IDictionary<string, BsonType> conversions;
    static ProjectionMongoDBCopyCollectionName collection_name;
    Establish context = () =>
    {
        conversions = new Dictionary<string, BsonType>();
        collection_name = "some_collection";
        conversions_resolver
            .Setup(_ => _.TryResolve<projection_type_with_mongo_db_copy_and_collection_name>(Moq.It.IsAny<IClientBuildResults>(), out conversions))
            .Returns(true);
        collection_name_validator
            .Setup(_ => _.Validate(Moq.It.IsAny<IClientBuildResults>(), Moq.It.IsAny<ProjectionMongoDBCopyCollectionName>()))
            .Returns(true);
    };
    
    Because of = () => succeeded = copy_adder.TryAugment<projection_type_with_mongo_db_copy_and_collection_name>(build_results, projection_copies, out augmented_result);

    It should_succeed = () => succeeded.ShouldBeTrue();
    It should_call_resolve_conversions_once = () => conversions_resolver.Verify(_ => _.TryResolve<projection_type_with_mongo_db_copy_and_collection_name>(build_results, out Moq.It.Ref<IDictionary<string, BsonType>>.IsAny), Times.Once);
    It should_validate_collection_name = () => collection_name_validator.Verify(_ => _.Validate(build_results, collection_name), Times.Once);
    It should_output_augmented_result = () => augmented_result.ShouldNotBeNull();
    It should_create_mongo_db_copy_definition = () => augmented_result.MongoDB.ShouldNotBeNull();
    It should_have_the_correct_collection_name = () => augmented_result.MongoDB.CollectionName.ShouldEqual(collection_name);
    It should_have_the_correct_conversions = () => augmented_result.MongoDB.Conversions.ShouldEqual(conversions);
    It should_not_result_in_failed_build_results = () => build_results.Failed.ShouldBeFalse();
}
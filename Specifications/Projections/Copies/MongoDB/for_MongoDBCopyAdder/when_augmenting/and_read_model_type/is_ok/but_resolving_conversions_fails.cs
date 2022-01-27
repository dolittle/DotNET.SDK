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

public class but_resolving_conversions_fails : given.all_dependencies
{
    static IDictionary<string, BsonType> conversions;
    Establish context = () =>
    {
        conversions = null;
        conversions_resolver
            .Setup(_ => _.TryResolve<projection_type_with_mongo_db_copy>(Moq.It.IsAny<IClientBuildResults>(), out conversions))
            .Returns(false);
        collection_name_validator
            .Setup(_ => _.Validate(Moq.It.IsAny<IClientBuildResults>(), Moq.It.IsAny<ProjectionMongoDBCopyCollectionName>()))
            .Returns(true);
    };
    Because of = () => succeeded = copy_adder.TryAugment<projection_type_with_mongo_db_copy>(build_results, projection_copies, out augmented_result);

    It should_not_succeed = () => succeeded.ShouldBeFalse();
    It should_not_output_augmented_result = () => augmented_result.ShouldBeNull();
    It should_call_resolve_conversions_once = () => conversions_resolver.Verify(_ => _.TryResolve<projection_type_with_mongo_db_copy>(build_results, out Moq.It.Ref<IDictionary<string, BsonType>>.IsAny), Times.Once);
    It should_result_in_failed_build_results = () => build_results.Failed.ShouldBeTrue();
}
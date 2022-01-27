// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Projections.Copies.MongoDB.for_MongoDBCopyAdder.given;
using Machine.Specifications;

namespace Dolittle.SDK.Projections.Copies.MongoDB.for_MongoDBCopyAdder.when_augmenting.and_read_model_type;

public class is_missing_copy_to_mongo_decorator : given.all_dependencies
{
    Because of = () => succeeded = copy_adder.TryAugment<projection_type_without_mongo_db_copy>(build_results, projection_copies, out augmented_result);

    It should_not_succeed = () => succeeded.ShouldBeFalse();
    It should_not_output_augmented_result = () => augmented_result.ShouldBeNull();
    It should_result_in_failed_build_results = () => build_results.Failed.ShouldBeTrue();
}
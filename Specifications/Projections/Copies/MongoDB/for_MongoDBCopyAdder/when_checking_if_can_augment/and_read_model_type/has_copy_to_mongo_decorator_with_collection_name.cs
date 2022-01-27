// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.SDK.Projections.Copies.MongoDB.for_MongoDBCopyAdder.when_checking_if_can_augment.and_read_model_type;

public class has_copy_to_mongo_decorator_with_collection_name : given.all_dependencies
{
    static bool succeeded;

    Because of = () => succeeded = copy_adder.CanAugment<given.projection_type_with_mongo_db_copy_and_collection_name>();

    It should_succeed = () => succeeded.ShouldBeTrue();
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.Projections.Copies.MongoDB.for_MongoDbCopyDefinitionFromReadModelBuilder.when_checking_if_can_build_from.and_read_model_type;

public class has_copy_to_mongo_decorator : given.all_dependencies
{
    static bool succeeded;

    Because of = () => succeeded = CopyDefinitionFromReadModelBuilder.CanBuildFrom<given.projection_type_with_mongo_db_copy>();

    It should_succeed = () => succeeded.ShouldBeTrue();
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.SDK.Projections.Copies.MongoDB.for_MongoDbCollectionNameValidator.when_validating.and_collection_name;

public class begins_with_system_dot : given.a_validator
{
    static bool result;
    Because of = () => result = validator.Validate(identifier, build_results, "system.name");

    It should_fail = () => result.ShouldBeFalse();
    It should_result_in_failing_build_results = () => build_results.Failed.ShouldBeTrue();
}
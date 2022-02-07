using System.Linq;
using Machine.Specifications;

namespace Dolittle.SDK.Projections.Copies.MongoDB.for_MongoDbCollectionNameValidator.when_validating.and_collection_name;

public class is_64_characters : given.a_validator
{
    static bool result;
    Because of = () => result = validator.Validate(build_results, string.Concat(Enumerable.Repeat('a', 64)));

    It should_fail = () => result.ShouldBeFalse();
    It should_result_in_failing_build_results = () => build_results.Failed.ShouldBeTrue();
}
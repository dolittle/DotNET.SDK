using Machine.Specifications;

namespace Dolittle.SDK.Projections.Copies.MongoDB.for_MongoDbCollectionNameValidator.when_validating.and_collection_name;

public class contains_the_dollar_character : given.a_validator
{
    static bool result;
    Because of = () => result = validator.Validate(build_results, "collection$name");

    It should_fail = () => result.ShouldBeFalse();
    It should_result_in_failing_build_results = () => build_results.Failed.ShouldBeTrue();
}
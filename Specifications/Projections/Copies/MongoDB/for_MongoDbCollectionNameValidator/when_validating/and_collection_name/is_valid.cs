using Machine.Specifications;

namespace Dolittle.SDK.Projections.Copies.MongoDB.for_MongoDbCollectionNameValidator.when_validating.and_collection_name;

public class is_valid : given.a_validator
{
    static bool result;
    
    Because of = () => result = validator.Validate(build_results, "a_fine_collection_name");

    It should_succeed = () => result.ShouldBeTrue();
    It should_not_result_in_failing_build_results = () => build_results.Failed.ShouldBeFalse();

}
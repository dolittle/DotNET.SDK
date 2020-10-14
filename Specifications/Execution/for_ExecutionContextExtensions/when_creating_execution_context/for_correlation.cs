// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.SDK.Execution.for_ExecutionContextExtensions.when_creating_execution_context
{
    public class for_correlation : given.an_execution_context
    {
        static CorrelationId new_correlation;
        static ExecutionContext result;

        Establish context = () => new_correlation = "f03cdf39-72c0-4e18-a1d5-37beb2ba972f";
        Because of = () => result = execution_context.ForCorrelation(new_correlation);

        It should_not_be_the_same_as_the_original_execution_context = () => result.ShouldNotEqual(execution_context);
        It should_have_the_correct_microservice = () => result.Microservice.ShouldEqual(microservice);
        It should_have_the_correct_tenant = () => result.Tenant.ShouldEqual(tenant);
        It should_have_the_correct_version = () => result.Version.ShouldEqual(version);
        It should_have_the_correct_environment = () => result.Environment.ShouldEqual(environment);
        It should_have_the_new_correlation_id = () => result.CorrelationId.ShouldEqual(new_correlation);
        It should_have_the_correct_claims = () => result.Claims.ShouldEqual(claims);
        It should_have_the_correct_culture = () => result.Culture.ShouldEqual(culture_info);
    }
}
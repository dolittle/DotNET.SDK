// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Security;
using Machine.Specifications;

namespace Dolittle.SDK.Execution.for_ExecutionContextExtensions.when_creating_execution_context
{
    public class for_claims : given.an_execution_context
    {
        static Claims new_claims;
        static ExecutionContext result;

        Establish context = () => new_claims = new Claims(new[] { new Claim("some other name", "some other value", "some other value type") });
        Because of = () => result = execution_context.ForClaims(new_claims);

        It should_not_be_the_same_as_the_original_execution_context = () => result.ShouldNotEqual(execution_context);
        It should_have_the_same_microservice = () => result.Microservice.ShouldEqual(microservice);
        It should_have_the_same_tenant = () => result.Tenant.ShouldEqual(tenant);
        It should_have_the_same_version = () => result.Version.ShouldEqual(version);
        It should_have_the_same_environment = () => result.Environment.ShouldEqual(environment);
        It should_have_the_same_correlation_id = () => result.CorrelationId.ShouldEqual(correlation_id);
        It should_have_the_new_claims = () => result.Claims.ShouldEqual(new_claims);
        It should_have_the_same_culture = () => result.Culture.ShouldEqual(culture_info);
    }
}
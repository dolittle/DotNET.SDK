// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Tenancy;
using Machine.Specifications;

namespace Dolittle.SDK.Execution.for_ExecutionContextExtensions.when_creating_execution_context
{
    public class for_tenant : given.an_execution_context
    {
        static TenantId new_tenant;
        static ExecutionContext result;

        Establish context = () => new_tenant = "27ba614a-bbeb-4801-b341-8ce2a7eecf80";
        Because of = () => result = execution_context.ForTenant(new_tenant);

        It should_not_be_the_same_as_the_original_execution_context = () => result.ShouldNotEqual(execution_context);
        It should_have_the_correct_microservice = () => result.Microservice.ShouldEqual(microservice);
        It should_have_the_new_tenant = () => result.Tenant.ShouldEqual(new_tenant);
        It should_have_the_correct_version = () => result.Version.ShouldEqual(version);
        It should_have_the_correct_environment = () => result.Environment.ShouldEqual(environment);
        It should_have_the_correct_correlation_id = () => result.CorrelationId.ShouldEqual(correlation_id);
        It should_have_the_correct_claims = () => result.Claims.ShouldEqual(claims);
        It should_have_the_correct_culture = () => result.Culture.ShouldEqual(culture_info);
    }
}
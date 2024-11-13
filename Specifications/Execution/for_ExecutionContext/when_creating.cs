// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Globalization;
using Dolittle.SDK.Microservices;
using Dolittle.SDK.Security;
using Dolittle.SDK.Tenancy;
using Machine.Specifications;

namespace Dolittle.SDK.Execution.for_ExecutionContext;

public class when_creating
{
    static MicroserviceId microservice;
    static TenantId tenant;
    static Version version;
    static Environment environment;
    static CorrelationId correlation_id;
    static Claims claims;
    static CultureInfo culture_info;
    static ExecutionContext result;
    static ActivitySpanId span_id;

    Establish context = () =>
    {
        microservice = "d357fe51-c263-484c-8051-e7fb4af7c340";
        tenant = "37c2138a-f1e0-4463-9b74-89d477cbe902";
        version = new Version(3, 2, 1, 23, "pre-release");
        environment = "some environment";
        correlation_id = "d1fa2359-52a9-479f-9b6d-72c6f72d7c3b";
        claims = new Claims([new Claim("some name", "some value", "some value type")]);
        culture_info = CultureInfo.InvariantCulture;
        span_id = ActivitySpanId.CreateFromString("cafecafecafecafe");
    };

    Because of = () => result = new ExecutionContext(microservice, tenant, version, environment, correlation_id, claims, culture_info, span_id);

    It should_have_the_correct_microservice = () => result.Microservice.ShouldEqual(microservice);
    It should_have_the_correct_tenant = () => result.Tenant.ShouldEqual(tenant);
    It should_have_the_correct_version = () => result.Version.ShouldEqual(version);
    It should_have_the_correct_environment = () => result.Environment.ShouldEqual(environment);
    It should_have_the_correct_correlation_id = () => result.CorrelationId.ShouldEqual(correlation_id);
    It should_have_the_correct_claims = () => result.Claims.ShouldEqual(claims);
    It should_have_the_correct_culture = () => result.Culture.ShouldEqual(culture_info);
    It should_have_the_correct_span_id = () => result.SpanId.ShouldEqual(span_id);
}
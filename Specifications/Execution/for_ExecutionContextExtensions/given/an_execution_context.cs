// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using Dolittle.SDK.Microservices;
using Dolittle.SDK.Security;
using Dolittle.SDK.Tenancy;
using Machine.Specifications;

namespace Dolittle.SDK.Execution.for_ExecutionContextExtensions.given;

public class an_execution_context
{
    protected static MicroserviceId microservice;
    protected static TenantId tenant;
    protected static Version version;
    protected static Environment environment;
    protected static CorrelationId correlation_id;
    protected static Claims claims;
    protected static CultureInfo culture_info;
    protected static ExecutionContext execution_context;

    Establish context = () =>
    {
        microservice = "d357fe51-c263-484c-8051-e7fb4af7c340";
        tenant = "37c2138a-f1e0-4463-9b74-89d477cbe902";
        version = new Version(3, 2, 1, 23, "pre-release");
        environment = "some environment";
        correlation_id = "d1fa2359-52a9-479f-9b6d-72c6f72d7c3b";
        claims = new Claims(new[] { new Claim("some name", "some value", "some value type") });
        culture_info = CultureInfo.InvariantCulture;
        execution_context = new ExecutionContext(microservice, tenant, version, environment, correlation_id, claims, culture_info);
    };
}
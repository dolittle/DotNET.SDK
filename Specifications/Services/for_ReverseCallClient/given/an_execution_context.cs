// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Microservices;
using Dolittle.SDK.Security;
using Dolittle.SDK.Tenancy;
using Machine.Specifications;

namespace Dolittle.SDK.Services.for_ReverseCallClient.given
{
    public class an_execution_context : a_test_scheduler
    {
        protected static ExecutionContext executionContext;

        Establish context = () =>
        {
            executionContext = new ExecutionContext(
                MicroserviceId.New(),
                TenantId.Development,
                Version.NotSet,
                Environment.Undetermined,
                CorrelationId.New(),
                Claims.Empty,
                CultureInfo.InvariantCulture);
        };
    }
}
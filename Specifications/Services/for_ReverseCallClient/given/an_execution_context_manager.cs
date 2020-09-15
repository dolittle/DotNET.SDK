// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Microservices;
using Dolittle.SDK.Security;
using Dolittle.SDK.Tenancy;
using Machine.Specifications;
using Moq;

namespace Dolittle.SDK.Services.for_ReverseCallClient.given
{
    public class an_execution_context_manager
    {
        protected static IExecutionContextManager executionContextManager;

        Establish context = () =>
        {
            var mock = new Mock<IExecutionContextManager>();
            mock.SetupGet(_ => _.Current)
                .Returns(new ExecutionContext(
                    MicroserviceId.New(),
                    TenantId.Development,
                    Version.NotSet,
                    Environment.Undetermined,
                    CorrelationId.New(),
                    Claims.Empty,
                    CultureInfo.InvariantCulture));
            executionContextManager = mock.Object;
        };
    }
}
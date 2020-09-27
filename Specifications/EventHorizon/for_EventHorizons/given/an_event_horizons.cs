// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using System.Threading;
using Dolittle.SDK.Microservices;
using Dolittle.SDK.Security;
using Dolittle.SDK.Services;
using Machine.Specifications;
using Microsoft.Extensions.Logging;
using Moq;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.EventHorizon.for_EventHorizons.given
{
    public class an_event_horizons
    {
        protected static CancellationTokenSource cancellation_token_source;
        protected static Mock<IPerformMethodCalls> caller;
        protected static ExecutionContext execution_context;
        protected static EventHorizons event_horizons;

        Establish context = () =>
        {
            cancellation_token_source = new CancellationTokenSource();

            caller = new Mock<IPerformMethodCalls>();

            execution_context = new ExecutionContext(
                "88e8ef99-8594-4108-b2ad-bb76f8b36233",
                "e575e5b8-0519-4859-8136-665cd399acc9",
                new Version(9, 8, 7, 1000, "bihpojekec"),
                "agdafuubge",
                "6b83676e-e6d6-45ad-bab0-db183120a3dd",
                new Claims(new[]
                    {
                        new Claim("kesoifzatf", "pupboowuep", "jihisgocru"),
                        new Claim("tazgecmiuc", "ridpezvado", "achizuwrag"),
                    }),
                CultureInfo.InvariantCulture);

            var logger = Mock.Of<ILogger>();

            event_horizons = new EventHorizons(caller.Object, execution_context, logger, cancellation_token_source.Token);
        };
    }
}

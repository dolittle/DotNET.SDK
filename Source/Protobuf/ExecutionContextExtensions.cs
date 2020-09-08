// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Microservices;
using Dolittle.SDK.Tenancy;
using PbExecutionContext = Dolittle.Execution.Contracts.ExecutionContext;

namespace Dolittle.SDK.Protobuf
{
    /// <summary>
    /// Conversion extensions for converting between <see cref="ExecutionContext"/> and <see cref="PbExecutionContext"/>.
    /// </summary>
    public static class ExecutionContextExtensions
    {
        /// <summary>
        /// Convert a <see cref="ExecutionContext"/> to <see cref="PbExecutionContext"/>.
        /// </summary>
        /// <param name="executionContext"><see cref="ExecutionContext"/> to convert from.</param>
        /// <returns>Converted <see cref="PbExecutionContext"/>.</returns>
        public static PbExecutionContext ToProtobuf(this ExecutionContext executionContext)
        {
            var message = new PbExecutionContext
                {
                    MicroserviceId = executionContext.Microservice.ToProtobuf(),
                    TenantId = executionContext.Tenant.ToProtobuf(),
                    CorrelationId = executionContext.CorrelationId.ToProtobuf(),
                    Environment = executionContext.Environment,
                    Version = executionContext.Version.ToProtobuf(),
                };
            message.Claims.AddRange(executionContext.Claims.ToProtobuf());
            return message;
        }

        /// <summary>
        /// Convert a <see cref="PbExecutionContext"/> to <see cref="ExecutionContext"/>.
        /// </summary>
        /// <param name="executionContext"><see cref="PbExecutionContext"/> to convert from.</param>
        /// <returns>Converted <see cref="ExecutionContext"/>.</returns>
        public static ExecutionContext ToExecutionContext(this PbExecutionContext executionContext) =>
            new ExecutionContext(
                executionContext.MicroserviceId.To<MicroserviceId>(),
                executionContext.TenantId.To<TenantId>(),
                executionContext.Version.ToVersion(),
                executionContext.Environment,
                executionContext.CorrelationId.To<CorrelationId>(),
                executionContext.Claims.ToClaims(),
                CultureInfo.InvariantCulture);
    }
}
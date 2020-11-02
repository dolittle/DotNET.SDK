// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
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
        /// <param name="source"><see cref="PbExecutionContext"/> to convert from.</param>
        /// <param name="executionContext">When the method returns, the converted <see cref="ExecutionContext"/> if conversion was successful, otherwise null.</param>
        /// <param name="error">When the method returns, null if the conversion was successful, otherwise the error that caused the failure.</param>
        /// <returns>A value indicating whether or not the conversion was successful.</returns>
        public static bool TryToExecutionContext(this PbExecutionContext source, out ExecutionContext executionContext, out Exception error)
        {
            executionContext = default;

            if (source == default)
            {
                error = new ArgumentNullException(nameof(source));
                return false;
            }

            if (!source.MicroserviceId.TryTo<MicroserviceId>(out var microserviceId, out var microserviceError))
            {
                error = new InvalidExecutionContextInformation(nameof(source.MicroserviceId), microserviceError);
                return false;
            }

            if (!source.TenantId.TryTo<TenantId>(out var tenantId, out var tenantError))
            {
                error = new InvalidExecutionContextInformation(nameof(source.MicroserviceId), tenantError);
                return false;
            }

            if (source.Version == default)
            {
                error = new MissingExecutionContextInformation(nameof(source.Version));
                return false;
            }

            if (!source.CorrelationId.TryTo<CorrelationId>(out var correlationId, out var correlationError))
            {
                error = new InvalidExecutionContextInformation(nameof(source.CorrelationId), correlationError);
                return false;
            }

            if (!source.Claims.TryToClaims(out var claims, out var claimsError))
            {
                error = new InvalidExecutionContextInformation(nameof(source.Claims), claimsError);
                return false;
            }

            executionContext = new ExecutionContext(
                microserviceId,
                tenantId,
                source.Version.ToVersion(),
                source.Environment,
                correlationId,
                claims,
                CultureInfo.InvariantCulture);
            error = null;
            return true;
        }

        /// <summary>
        /// Convert a <see cref="PbExecutionContext"/> to <see cref="ExecutionContext"/>.
        /// </summary>
        /// <param name="source"><see cref="PbExecutionContext"/> to convert from.</param>
        /// <returns>Converted <see cref="ExecutionContext"/>.</returns>
        public static ExecutionContext ToExecutionContext(this PbExecutionContext source)
            => source.TryToExecutionContext(out var executionContext, out var error) ? executionContext : throw error;
    }
}
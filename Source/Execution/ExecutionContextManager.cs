// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;
using Dolittle.SDK.Microservices;
using Dolittle.SDK.Security;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Execution
{
    /// <summary>
    /// Represents an implementation of <see cref="IExecutionContextManager"/>.
    /// </summary>
    public class ExecutionContextManager : IExecutionContextManager
    {
        static readonly AsyncLocal<ExecutionContext> _executionContext = new AsyncLocal<ExecutionContext>();

        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionContextManager"/> class.
        /// </summary>
        /// <param name="microserviceId">Which <see cref="MicroserviceId"/> that is running.</param>
        /// <param name="version">The <see cref="Version" /> of the <see cref="MicroserviceId" />.</param>
        /// <param name="environment">Which <see cref="Environment"/> the system is running in.</param>
        /// <param name="logger">The <see cref="ILogger"/> for logging.</param>
        public ExecutionContextManager(MicroserviceId microserviceId, Version version, Environment environment, ILogger logger)
        {
            _logger = logger;
            Current = new ExecutionContext(
                microserviceId,
                TenantId.System,
                version,
                environment,
                CorrelationId.System,
                Claims.Empty,
                CultureInfo.InvariantCulture);
        }

        /// <inheritdoc/>
        public ExecutionContext Current
        {
            get
            {
                return _executionContext.Value;
            }

            private set
            {
                _executionContext.Value = value;
            }
        }

        /// <inheritdoc/>
        public IExecutionContextManager ForTenant(TenantId tenant, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string member = "")
        {
            _logger.LogTrace("Setting tenant in execution context ({tenant}) - from: ({filePath}, {lineNumber}, {member}) ", tenant, filePath, lineNumber, member);
            Current = new ExecutionContext(
                Current.Microservice,
                tenant,
                Current.Version,
                Current.Environment,
                Current.CorrelationId,
                Current.Claims,
                Current.Culture);
            return this;
        }

        /// <inheritdoc/>
        public IExecutionContextManager ForCorrelation(CorrelationId correlationId, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string member = "")
        {
            _logger.LogTrace("Setting correlation in execution context ({correlationId}) - from: ({filePath}, {lineNumber}, {member}) ", correlationId, filePath, lineNumber, member);
            Current = new ExecutionContext(
                Current.Microservice,
                Current.Tenant,
                Current.Version,
                Current.Environment,
                correlationId,
                Current.Claims,
                Current.Culture);
            return this;
        }

        /// <inheritdoc/>
        public IExecutionContextManager ForClaims(Claims claims, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string member = "")
        {
            _logger.LogTrace("Setting claims in execution context ({claims}) - from: ({filePath}, {lineNumber}, {member}) ", claims, filePath, lineNumber, member);
            Current = new ExecutionContext(
                Current.Microservice,
                Current.Tenant,
                Current.Version,
                Current.Environment,
                Current.CorrelationId,
                claims,
                Current.Culture);
            return this;
        }
    }
}

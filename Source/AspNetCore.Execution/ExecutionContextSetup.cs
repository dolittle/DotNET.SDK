// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Tenancy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Dolittle.AspNetCore.Execution
{
    /// <summary>
    /// Provides an endpoint for that sets the <see cref="TenantId"/> of the <see cref="ExecutionContext" />.
    /// </summary>
    public class ExecutionContextSetup
    {
        readonly RequestDelegate _next;
        readonly IOptionsMonitor<ExecutionContextSetupOptions> _options;
        readonly IExecutionContextManager _executionContextManager;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionContextSetup"/> class.
        /// </summary>
        /// <param name="next">Next middleware.</param>
        /// <param name="options">Configuration in form of <see cref="ExecutionContextSetupOptions"/>.</param>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for working with <see cref="ExecutionContext"/>.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public ExecutionContextSetup(
            RequestDelegate next,
            IOptionsMonitor<ExecutionContextSetupOptions> options,
            IExecutionContextManager executionContextManager,
            ILogger logger)
        {
            _next = next;
            _options = options;
            _executionContextManager = executionContextManager;
            _logger = logger;
        }

        /// <summary>
        /// Middleware invocation method.
        /// </summary>
        /// <param name="context">Current <see cref="HttpContext"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task InvokeAsync(HttpContext context)
        {
            var tenantIdHeader = _options.CurrentValue.TenantIdHeaderName;
            var tenantId = GetTenantIdFromHeaders(context, tenantIdHeader);

            _executionContextManager.CurrentFor(tenantId, CorrelationId.New());
            return _next.Invoke(context);
        }

        TenantId GetTenantIdFromHeaders(HttpContext context, string header)
        {
            var values = context.Request.Headers[header];
            ThrowIfTenantIdHeaderHasMultipleValues(header, values);
            if (values.Count == 1)
            {
                if (Guid.TryParse(values[0], out var tenantId))
                {
                    return tenantId;
                }
                else
                {
                    _logger.Error(
                        "The configured TenantId header '{Header}' must be a valid Guid. The value was '{FirstValue}' - no tenant will be configured",
                        header,
                        values[0]);
                }
            }

            return TenantId.Unknown;
        }

        void ThrowIfTenantIdHeaderHasMultipleValues(string header, StringValues values)
        {
            if (values.Count > 1)
            {
                throw new TenantIdHeaderHasMultipleValues(header);
            }
        }
    }
}
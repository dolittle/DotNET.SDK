// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.SDK.Events.Processing;
using Dolittle.SDK.Protobuf;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.Events.Filters.Internal
{
    /// <summary>
    /// Represents a <see cref="FilterEventProcessor{TRegisterArguments, TResponse}" /> that can filter non-partitioned private events.
    /// </summary>
    public class UnpartitionedEventFilterProcessor : FilterEventProcessor<FilterRegistrationRequest, FilterResponse>
    {
        readonly FilterEventCallback _filterEventCallback;
        readonly FilterId _filterId;
        readonly ScopeId _scopeId;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnpartitionedEventFilterProcessor"/> class.
        /// </summary>
        /// <param name="filterId">The <see cref="FilterId" />.</param>
        /// <param name="scopeId">The <see cref="ScopeId" />.</param>
        /// <param name="filterEventCallback">The <see cref="FilterEventCallback" />.</param>
        /// <param name="converter">The <see cref="IEventProcessingConverter" />.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
        public UnpartitionedEventFilterProcessor(
            FilterId filterId,
            ScopeId scopeId,
            FilterEventCallback filterEventCallback,
            IEventProcessingConverter converter,
            ILoggerFactory loggerFactory)
            : base("Unpartitioned Filter", filterId, converter, loggerFactory)
        {
            _filterId = filterId;
            _scopeId = scopeId;
            _filterEventCallback = filterEventCallback;
        }

        /// <inheritdoc/>
        public override FilterRegistrationRequest RegistrationRequest
            => new FilterRegistrationRequest
                {
                    FilterId = _filterId.ToProtobuf(),
                    ScopeId = _scopeId.ToProtobuf(),
                };

        /// <inheritdoc/>
        protected override FilterResponse CreateResponseFromFailure(ProcessorFailure failure)
            => new FilterResponse { Failure = failure };

        /// <inheritdoc/>
        protected override async Task<FilterResponse> Filter(object @event, EventContext context, ExecutionContext executionContext, CancellationToken cancellation)
            => new FilterResponse { IsIncluded = await _filterEventCallback(@event, context).ConfigureAwait(false) };
    }
}

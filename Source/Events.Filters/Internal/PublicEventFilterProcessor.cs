// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.SDK.Events.Processing;
using Dolittle.SDK.Protobuf;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Filters.Internal
{
    /// <summary>
    /// Represents a <see cref="FilterEventProcessor{TRegisterArguments, TResponse}" /> that can filter public events.
    /// </summary>
    public class PublicEventFilterProcessor : FilterEventProcessor<PublicFilterRegistrationRequest, PartitionedFilterResponse>
    {
        readonly PartitionedFilterEventCallback _filterEventCallback;
        readonly FilterId _filterId;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublicEventFilterProcessor"/> class.
        /// </summary>
        /// <param name="filterId">The <see cref="FilterId" />.</param>
        /// <param name="filterEventCallback">The <see cref="PartitionedFilterEventCallback" />.</param>
        /// <param name="converter">The <see cref="IEventProcessingConverter" />.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
        public PublicEventFilterProcessor(
            FilterId filterId,
            PartitionedFilterEventCallback filterEventCallback,
            IEventProcessingConverter converter,
            ILoggerFactory loggerFactory)
            : base("Public Filter", filterId, converter, loggerFactory)
        {
            _filterEventCallback = filterEventCallback;
            _filterId = filterId;
        }

        /// <inheritdoc/>
        public override PublicFilterRegistrationRequest RegistrationRequest
            => new PublicFilterRegistrationRequest
                {
                    FilterId = _filterId.ToProtobuf()
                };

        /// <inheritdoc/>
        protected override PartitionedFilterResponse CreateResponseFromFailure(ProcessorFailure failure)
            => new PartitionedFilterResponse { Failure = failure };

        /// <inheritdoc/>
        protected override async Task<PartitionedFilterResponse> Filter(object @event, EventContext context, CancellationToken cancellation)
        {
            var result = await _filterEventCallback(@event, context).ConfigureAwait(false);
            return new PartitionedFilterResponse { IsIncluded = result.ShouldInclude, PartitionId = result.PartitionId.ToProtobuf() };
        }
    }
}
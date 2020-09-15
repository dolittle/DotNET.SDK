// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.SDK.Events.Processing;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Filters.Internal
{
    /// <summary>
    /// Represents a <see cref="FilterEventProcessor{TRegisterArguments, TResponse}" /> that can filter non-partitioned private events.
    /// </summary>
    public class EventFilterProcessor : FilterEventProcessor<FilterRegistrationRequest, FilterResponse>
    {
        readonly FilterEventCallback _filterEventCallback;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventFilterProcessor"/> class.
        /// </summary>
        /// <param name="filterId">The <see cref="FilterId" />.</param>
        /// <param name="filterEventCallback">The <see cref="FilterEventCallback" />.</param>
        /// <param name="processingRequestConverter">The <see cref="IEventProcessingRequestConverter" />.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
        public EventFilterProcessor(
            FilterId filterId,
            FilterEventCallback filterEventCallback,
            IEventProcessingRequestConverter processingRequestConverter,
            ILoggerFactory loggerFactory)
            : base(Kind, filterId, processingRequestConverter, loggerFactory)
        {
            _filterEventCallback = filterEventCallback;
        }

        /// <summary>
        /// Gets the <see cref="EventProcessorKind" />.
        /// </summary>
        public static EventProcessorKind Kind => "Filter";

        /// <inheritdoc/>
        protected override FilterResponse CreateResponseFromFailure(ProcessorFailure failure)
            => new FilterResponse { Failure = failure };

        /// <inheritdoc/>
        protected override async Task<FilterResponse> Filter(object @event, EventContext context)
            => new FilterResponse { IsIncluded = await _filterEventCallback(@event, context).ConfigureAwait(false) };
    }
}

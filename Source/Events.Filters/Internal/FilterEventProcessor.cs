// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.SDK.Events.Processing;
using Dolittle.SDK.Events.Processing.Internal;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Filters.Internal
{
    /// <summary>
    /// Represents an <see cref="EventProcessor{TIdentifier, TRegisterArguments, TRegisterResponse, TRequest, TResponse}" /> for an event filter.
    /// </summary>
    /// <typeparam name="TRegisterArguments">The <see cref="System.Type" /> of the registration arguments.</typeparam>
    /// <typeparam name="TResponse">The <see cref="System.Type" /> of the response.</typeparam>
    public abstract class FilterEventProcessor<TRegisterArguments, TResponse> : EventProcessor<FilterId, TRegisterArguments, FilterRegistrationResponse, FilterEventRequest, TResponse>
        where TRegisterArguments : class
        where TResponse : class
    {
        readonly IEventProcessingRequestConverter _processingRequestConverter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterEventProcessor{TRegisterArguments, TResponse}"/> class.
        /// </summary>
        /// <param name="kind">The kind of the <see cref="FilterEventProcessor{TRegisterArguments, TResponse}" />.</param>
        /// <param name="filterId">The <see cref="FilterId" />.</param>
        /// <param name="processingRequestConverter">The <see cref="IEventProcessingRequestConverter" />.</param>
        /// <param name="loggerfactory">The <see cref="ILoggerFactory" />.</param>
        protected FilterEventProcessor(
            EventProcessorKind kind,
            FilterId filterId,
            IEventProcessingRequestConverter processingRequestConverter,
            ILoggerFactory loggerfactory)
            : base(kind, filterId, loggerfactory.CreateLogger<EventProcessor<FilterId, TRegisterArguments, FilterRegistrationResponse, FilterEventRequest, TResponse>>())
        {
            _processingRequestConverter = processingRequestConverter;
        }

        /// <inheritdoc/>
        protected override Task<TResponse> Process(FilterEventRequest request, CancellationToken cancellation)
        {
            var eventContext = _processingRequestConverter.GetEventContext(request.Event);
            var @event = _processingRequestConverter.GetCLREvent(request.Event);

            return Filter(@event, eventContext);
        }

        /// <summary>
        /// Filters an event.
        /// </summary>
        /// <param name="event">The event to filter.</param>
        /// <param name="context">The <see cref="EventContext" />.</param>
        /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns a <typeparamref name="TResponse"/>.</returns>
        protected abstract Task<TResponse> Filter(object @event, EventContext context);

        /// <inheritdoc/>
        protected override RetryProcessingState GetRetryProcessingStateFromRequest(FilterEventRequest request)
            => request.RetryProcessingState;
    }
}

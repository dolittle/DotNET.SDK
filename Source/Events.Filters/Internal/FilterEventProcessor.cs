// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.SDK.Events.Processing;
using Dolittle.SDK.Events.Processing.Internal;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.Events.Filters.Internal
{
    /// <summary>
    /// Represents an <see cref="EventProcessor{TIdentifier, TRegisterArguments, TRequest, TResponse}" /> for an event filter.
    /// </summary>
    /// <typeparam name="TRegisterArguments">The <see cref="System.Type" /> of the registration arguments.</typeparam>
    /// <typeparam name="TResponse">The <see cref="System.Type" /> of the response.</typeparam>
    public abstract class FilterEventProcessor<TRegisterArguments, TResponse> : EventProcessor<FilterId, TRegisterArguments, FilterEventRequest, TResponse>, IFilterProcessor<TRegisterArguments, TResponse>
        where TRegisterArguments : class
        where TResponse : class
    {
        readonly IEventProcessingConverter _converter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterEventProcessor{TRegisterArguments, TResponse}"/> class.
        /// </summary>
        /// <param name="kind">The kind of the <see cref="FilterEventProcessor{TRegisterArguments, TResponse}" />.</param>
        /// <param name="filterId">The <see cref="FilterId" />.</param>
        /// <param name="converter">The <see cref="IEventProcessingConverter" />.</param>
        /// <param name="loggerfactory">The <see cref="ILoggerFactory" />.</param>
        protected FilterEventProcessor(
            EventProcessorKind kind,
            FilterId filterId,
            IEventProcessingConverter converter,
            ILoggerFactory loggerfactory)
            : base(kind, filterId, loggerfactory.CreateLogger<EventProcessor<FilterId, TRegisterArguments, FilterEventRequest, TResponse>>())
        {
            _converter = converter;
        }

        /// <inheritdoc/>
        protected override Task<TResponse> Process(FilterEventRequest request, ExecutionContext executionContext, CancellationToken cancellation)
        {
            var comittedEvent = _converter.ToSDK(request.Event);
            return Filter(comittedEvent.Content, comittedEvent.GetEventContext(), executionContext, cancellation);
        }

        /// <summary>
        /// Filters an event.
        /// </summary>
        /// <param name="event">The event to filter.</param>
        /// <param name="context">The <see cref="EventContext" />.</param>
        /// <param name="executionContext">The execution context to handle the request in.</param>
        /// <param name="cancellation">The <see cref="CancellationToken" /> used to cancel the processing of the request.</param>
        /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns a <typeparamref name="TResponse"/>.</returns>
        protected abstract Task<TResponse> Filter(object @event, EventContext context, ExecutionContext executionContext, CancellationToken cancellation);

        /// <inheritdoc/>
        protected override RetryProcessingState GetRetryProcessingStateFromRequest(FilterEventRequest request)
            => request.RetryProcessingState;
    }
}

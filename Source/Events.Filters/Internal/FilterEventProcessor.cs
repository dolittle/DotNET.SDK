// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events.Processing.Internal;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.SDK.Services;
using System;
using System.Threading.Tasks;
using System.Threading;
using Dolittle.Protobuf.Contracts;
using Microsoft.Extensions.Logging;
using Dolittle.SDK.Events.Processing;
using Dolittle.SDK.Protobuf;

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
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterEventProcessor{TRegisterArguments, TResponse}"/> class.
        /// </summary>
        /// <param name="kind">The kind of the <see cref="FilterEventProcessor{TRegisterArguments, TResponse}" />.</param>
        /// <param name="filterId">The <see cref="FilterId" />.</param>
        /// <param name="eventTypes">The <see cref="EventTypes" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        protected FilterEventProcessor(string kind, FilterId filterId, EventTypes eventTypes, ILogger logger)
            : base(kind, filterId, logger)
        {
        }

        /// <inheritdoc/>
        protected override Failure GetFailureFromRegisterResponse(FilterRegistrationResponse response) => response.Failure;

        /// <inheritdoc/>
        protected override RetryProcessingState GetRetryProcessingStateFromRequest(FilterEventRequest request) => request.RetryProcessingState;

        /// <inheritdoc/>
        protected override Task<FilterResponse> Handle(FilterEventRequest request)
        {
            var pbEvent = request.Event;
            if (pbEvent == default) throw new MissingEventInformation("No event in FilterEventRequest");

            EventLogSequenceNumber sequenceNumber = pbEvent.EventLogSequenceNumber;
            var eventSourceId = pbEvent.EventSourceId.To<EventSourceId>();
            var executionContext = pbEvent.ExecutionContext?.ToExecutionContext();
            var occurred = pbEvent.Occurred?.ToDateTimeOffset();
            var type = pbEvent.Type.ToEventType();
        }

        /// <summary>
        /// Filters an event.
        /// </summary>
        /// <param name="event">The event to filter.</param>
        /// <param name="context">The <see cref="EventContext" />.</param>
        /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns a <typeparamref name="TResponse"/>.</returns>
        protected abstract Task<TResponse> Filter(object @event, EventContext context);
    }
}
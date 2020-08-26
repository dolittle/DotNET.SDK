// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Events.Processing.Internal;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Services;
using Google.Protobuf;
using Failure = Dolittle.Protobuf.Contracts.Failure;

namespace Dolittle.Events.Filters.Internal
{
    /// <summary>
    /// Implementation of <see cref="EventProcessor{TIdentifier, TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}"/> for Event Filters.
    /// </summary>
    /// <typeparam name="TEventType">The event type that the filter can handle.</typeparam>
    /// <typeparam name="TClientMessage">Type of the <see cref="IMessage">messages</see> that is sent from the client to the server.</typeparam>
    /// <typeparam name="TRegistrationRequest">Type of the arguments that are sent along with the initial Connect call.</typeparam>
    /// <typeparam name="TFilterResponse">Type of the responses received from the client using <see cref="IReverseCallDispatcher{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}.Call"/>.</typeparam>
    public abstract class AbstractFilterProcessor<TEventType, TClientMessage, TRegistrationRequest, TFilterResponse> : EventProcessor<FilterId, TClientMessage, FilterRuntimeToClientMessage, TRegistrationRequest, FilterRegistrationResponse, FilterEventRequest, TFilterResponse>
        where TEventType : IEvent
        where TClientMessage : IMessage, new()
        where TRegistrationRequest : class
        where TFilterResponse : class
    {
        readonly IEventConverter _converter;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractFilterProcessor{TEventType, TClientMessage, TRegistrationRequest, TFilterResponse}"/> class.
        /// </summary>
        /// <param name="filterId">The unique <see cref="FilterId"/> for the event filter.</param>
        /// <param name="converter">The <see cref="IEventConverter"/> to use to convert events.</param>
        /// <param name="logger">The <see cref="ILogger"/> to use for logging.</param>
        protected AbstractFilterProcessor(
            FilterId filterId,
            IEventConverter converter,
            ILogger logger)
            : base(logger)
        {
            Identifier = filterId;
            _converter = converter;
        }

        /// <inheritdoc/>
        protected override FilterId Identifier { get; }

        /// <inheritdoc/>
        protected override Failure GetFailureFromRegisterResponse(FilterRegistrationResponse response)
            => response.Failure;

        /// <inheritdoc/>
        protected override RetryProcessingState GetRetryProcessingState(FilterEventRequest request)
            => request.RetryProcessingState;

        /// <inheritdoc/>
        protected override Task<TFilterResponse> Handle(FilterEventRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var committed = _converter.ToSDK(request.Event);
                if (committed.Event is TEventType typedEvent)
                {
                    return Filter(typedEvent, committed.DeriveContext());
                }

                throw new EventTypeIsIncorrectForFilter(typeof(TEventType), committed.Event.GetType());
            }
            catch (CouldNotDeserializeEvent ex)
            {
                throw new CouldNotDeserializeEventFromScope(request.ScopeId.To<ScopeId>(), ex);
            }
        }

        /// <summary>
        /// The method that will be called to invoke the filter with the event received from the Runtime.
        /// </summary>
        /// <param name="event">The <typeparamref name="TEventType"/> to filter.</param>
        /// <param name="context">The <see cref="EventContext"/> of the event to filter.</param>
        /// <returns>A <see cref="Task{TResponse}"/> that, when resolved, returns the <typeparamref name="TFilterResponse"/> to send back to the Runtime.</returns>
        protected abstract Task<TFilterResponse> Filter(TEventType @event, EventContext context);
    }
}
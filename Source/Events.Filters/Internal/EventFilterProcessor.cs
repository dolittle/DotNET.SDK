// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Services.Clients;
using static Dolittle.Runtime.Events.Processing.Contracts.Filters;

namespace Dolittle.Events.Filters.Internal
{
    /// <summary>
    /// An implementation of <see cref="AbstractFilterProcessor{TEventType, TClientMessage, TRegistrationRequest, TFilterResponse}"/> used for <see cref="ICanFilterEvents"/>.
    /// </summary>
    public class EventFilterProcessor : AbstractFilterProcessor<IEvent, FilterClientToRuntimeMessage, FilterRegistrationRequest, FilterResponse>
    {
        readonly ScopeId _scope;
        readonly FiltersClient _client;
        readonly IReverseCallClients _reverseCallClients;
        readonly ICanFilterEvents _filter;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventFilterProcessor"/> class.
        /// </summary>
        /// <param name="filterId">The unique <see cref="FilterId"/> for the event filter.</param>
        /// <param name="scope">The <see cref="ScopeId"/> of the scope in the Event Store where the filter will run.</param>
        /// <param name="client">The <see cref="FiltersClient"/> to use to connect to the Runtime.</param>
        /// <param name="reverseCallClients">The <see cref="IReverseCallClients"/> to use for creating instances of <see cref="IReverseCallClient{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}"/>.</param>
        /// <param name="filter">The <see cref="ICanFilterEvents"/> to use for filtering the events.</param>
        /// <param name="converter">The <see cref="IEventConverter"/> to use to convert events.</param>
        /// <param name="logger">The <see cref="ILogger"/> to use for logging.</param>
        public EventFilterProcessor(
            FilterId filterId,
            ScopeId scope,
            FiltersClient client,
            IReverseCallClients reverseCallClients,
            ICanFilterEvents filter,
            IEventConverter converter,
            ILogger logger)
            : base(filterId, converter, logger)
        {
            _scope = scope;
            _client = client;
            _reverseCallClients = reverseCallClients;
            _filter = filter;
        }

        /// <inheritdoc/>
        protected override string Kind => "filter";

        /// <inheritdoc/>
        protected override IReverseCallClient<FilterClientToRuntimeMessage, FilterRuntimeToClientMessage, FilterRegistrationRequest, FilterRegistrationResponse, FilterEventRequest, FilterResponse> CreateClient()
            => _reverseCallClients.GetFor<FilterClientToRuntimeMessage, FilterRuntimeToClientMessage, FilterRegistrationRequest, FilterRegistrationResponse, FilterEventRequest, FilterResponse>(
                () => _client.Connect(),
                (message, arguments) => message.RegistrationRequest = arguments,
                message => message.RegistrationResponse,
                message => message.FilterRequest,
                (message, response) => message.FilterResult = response,
                (arguments, context) => arguments.CallContext = context,
                request => request.CallContext,
                (response, context) => response.CallContext = context,
                message => message.Ping,
                (message, pong) => message.Pong = pong,
                TimeSpan.FromSeconds(5));

        /// <inheritdoc/>
        protected override FilterRegistrationRequest GetRegisterArguments()
            => new FilterRegistrationRequest
            {
                FilterId = Identifier.ToProtobuf(),
                ScopeId = _scope.ToProtobuf(),
            };

        /// <inheritdoc/>
        protected override FilterResponse CreateResponseFromFailure(ProcessorFailure failure)
            => new FilterResponse
            {
                Failure = failure,
            };

        /// <inheritdoc/>
        protected override async Task<FilterResponse> Filter(IEvent @event, EventContext context)
        {
            var result = await _filter.Filter(@event, context).ConfigureAwait(false);
            return new FilterResponse
            {
                IsIncluded = result.Included,
            };
        }
    }
}
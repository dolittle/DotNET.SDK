// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Events.Filters.EventHorizon;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Services.Clients;
using static Dolittle.Runtime.Events.Processing.Contracts.Filters;

namespace Dolittle.Events.Filters.Internal
{
    /// <summary>
    /// An implementation of <see cref="AbstractFilterProcessor{TEventType, TClientMessage, TRegistrationRequest, TFilterResponse}"/> used for <see cref="ICanFilterPublicEvents"/>.
    /// </summary>
    public class PublicEventFilterProcessor : AbstractFilterProcessor<IPublicEvent, PublicFilterClientToRuntimeMessage, PublicFilterRegistrationRequest, PartitionedFilterResponse>
    {
        readonly FiltersClient _client;
        readonly IReverseCallClients _reverseCallClients;
        readonly ICanFilterPublicEvents _filter;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublicEventFilterProcessor"/> class.
        /// </summary>
        /// <param name="filterId">The unique <see cref="FilterId"/> for the event filter.</param>
        /// <param name="client">The <see cref="FiltersClient"/> to use to connect to the Runtime.</param>
        /// <param name="reverseCallClients">The <see cref="IReverseCallClients"/> to use for creating instances of <see cref="IReverseCallClient{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}"/>.</param>
        /// <param name="filter">The <see cref="ICanFilterPublicEvents"/> to use for filtering the events.</param>
        /// <param name="converter">The <see cref="IEventConverter"/> to use to convert events.</param>
        /// <param name="logger">The <see cref="ILogger"/> to use for logging.</param>
        public PublicEventFilterProcessor(
            FilterId filterId,
            FiltersClient client,
            IReverseCallClients reverseCallClients,
            ICanFilterPublicEvents filter,
            IEventConverter converter,
            ILogger logger)
            : base(filterId, converter, logger)
        {
            _client = client;
            _reverseCallClients = reverseCallClients;
            _filter = filter;
        }

        /// <inheritdoc/>
        protected override string Kind => "public filter";

        /// <inheritdoc/>
        protected override IReverseCallClient<PublicFilterClientToRuntimeMessage, FilterRuntimeToClientMessage, PublicFilterRegistrationRequest, FilterRegistrationResponse, FilterEventRequest, PartitionedFilterResponse> CreateClient()
            => _reverseCallClients.GetFor<PublicFilterClientToRuntimeMessage, FilterRuntimeToClientMessage, PublicFilterRegistrationRequest, FilterRegistrationResponse, FilterEventRequest, PartitionedFilterResponse>(
                () => _client.ConnectPublic(),
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
        protected override PublicFilterRegistrationRequest GetRegisterArguments()
            => new PublicFilterRegistrationRequest
            {
                FilterId = Identifier.ToProtobuf(),
            };

        /// <inheritdoc/>
        protected override PartitionedFilterResponse CreateResponseFromFailure(ProcessorFailure failure)
            => new PartitionedFilterResponse
            {
                Failure = failure,
            };

        /// <inheritdoc/>
        protected override async Task<PartitionedFilterResponse> Filter(IPublicEvent @event, EventContext context)
        {
            var result = await _filter.Filter(@event, context).ConfigureAwait(false);
            return new PartitionedFilterResponse
            {
                IsIncluded = result.Included,
                PartitionId = result.Partition.ToProtobuf(),
            };
        }
    }
}
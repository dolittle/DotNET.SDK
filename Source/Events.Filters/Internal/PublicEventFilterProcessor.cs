// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.SDK.Protobuf;
using Dolittle.SDK.Services;
using Dolittle.Services.Contracts;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using static Dolittle.Runtime.Events.Processing.Contracts.Filters;

namespace Dolittle.SDK.Events.Filters.Internal
{
    /// <summary>
    /// Represents a <see cref="FilterEventProcessor{TRegisterArguments, TResponse}" /> that can filter public events.
    /// </summary>
    public class PublicEventFilterProcessor : FilterEventProcessor<PublicFilterRegistrationRequest, PartitionedFilterResponse>
    {
        readonly PartitionedFilterEventCallback _filterEventCallback;
        readonly FiltersClient _client;
        readonly ICreateReverseCallClients _reverseCallClientsCreator;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublicEventFilterProcessor"/> class.
        /// </summary>
        /// <param name="filterId">The <see cref="FilterId" />.</param>
        /// <param name="filterEventCallback">The <see cref="PartitionedFilterEventCallback" />.</param>
        /// <param name="client">The <see cref="FiltersClient" />.</param>
        /// <param name="reverseCallClientsCreator">The <see cref="ICreateReverseCallClients" />.</param>
        /// <param name="eventTypes">The <see cref="EventTypes" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public PublicEventFilterProcessor(
            FilterId filterId,
            PartitionedFilterEventCallback filterEventCallback,
            FiltersClient client,
            ICreateReverseCallClients reverseCallClientsCreator,
            EventTypes eventTypes,
            ILogger<EventFilterProcessor> logger)
            : base("Public Filter", filterId, eventTypes, logger)
        {
            _filterEventCallback = filterEventCallback;
            _client = client;
            _reverseCallClientsCreator = reverseCallClientsCreator;
        }

        /// <inheritdoc/>
        protected override PublicFilterRegistrationRequest RegisterArguments
            => new PublicFilterRegistrationRequest
                {
                    FilterId = Identifier.ToProtobuf()
                };

        /// <inheritdoc/>
        protected override IReverseCallClient<PublicFilterRegistrationRequest, FilterRegistrationResponse, FilterEventRequest, PartitionedFilterResponse> CreateClient(
            PublicFilterRegistrationRequest registerArguments,
            Func<FilterEventRequest, CancellationToken, Task<PartitionedFilterResponse>> callback,
            uint pingTimeout,
            CancellationToken cancellation)
            => _reverseCallClientsCreator.Create(
                RegisterArguments,
                this,
                new DuplexStreamingMethodCaller(_client),
                new ReverseCallMessageConverter());

        /// <inheritdoc/>
        protected override PartitionedFilterResponse CreateResponseFromFailure(ProcessorFailure failure)
            => new PartitionedFilterResponse { Failure = failure };

        /// <inheritdoc/>
        protected override async Task<PartitionedFilterResponse> Filter(object @event, EventContext context)
        {
            var result = await _filterEventCallback(@event, context).ConfigureAwait(false);
            return new PartitionedFilterResponse { IsIncluded = result.ShouldInclude, PartitionId = result.PartitionId.ToProtobuf() };
        }

        class DuplexStreamingMethodCaller : ICanCallADuplexStreamingMethod<FiltersClient, PublicFilterClientToRuntimeMessage, FilterRuntimeToClientMessage>
        {
            readonly FiltersClient _client;

            public DuplexStreamingMethodCaller(FiltersClient client)
            {
                _client = client;
            }

            public AsyncDuplexStreamingCall<PublicFilterClientToRuntimeMessage, FilterRuntimeToClientMessage> Call(Channel channel, CallOptions callOptions)
                => _client.ConnectPublic(callOptions);
        }

        class ReverseCallMessageConverter : IConvertReverseCallMessages<PublicFilterClientToRuntimeMessage, FilterRuntimeToClientMessage, PublicFilterRegistrationRequest, FilterRegistrationResponse, FilterEventRequest, PartitionedFilterResponse>
        {
            public PublicFilterClientToRuntimeMessage CreateMessageFrom(PublicFilterRegistrationRequest arguments)
                => new PublicFilterClientToRuntimeMessage { RegistrationRequest = arguments };

            public PublicFilterClientToRuntimeMessage CreateMessageFrom(Pong pong)
                => new PublicFilterClientToRuntimeMessage { Pong = pong };

            public PublicFilterClientToRuntimeMessage CreateMessageFrom(PartitionedFilterResponse response)
                => new PublicFilterClientToRuntimeMessage { FilterResult = response };

            public FilterRegistrationResponse GetConnectResponseFrom(FilterRuntimeToClientMessage message)
                => message.RegistrationResponse;

            public Ping GetPingFrom(FilterRuntimeToClientMessage message)
                => message.Ping;

            public ReverseCallRequestContext GetRequestContextFrom(FilterEventRequest message)
                => message.CallContext;

            public FilterEventRequest GetRequestFrom(FilterRuntimeToClientMessage message)
                => message.FilterRequest;

            public void SetConnectArgumentsContextIn(ReverseCallArgumentsContext context, PublicFilterRegistrationRequest arguments)
                => arguments.CallContext = context;

            public void SetResponseContextIn(ReverseCallResponseContext context, PartitionedFilterResponse response)
                => response.CallContext = context;
        }
    }
}
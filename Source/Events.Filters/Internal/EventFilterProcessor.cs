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
    /// Represents a <see cref="FilterEventProcessor{TRegisterArguments, TResponse}" /> that can filter non-partitioned private events.
    /// </summary>
    public class EventFilterProcessor : FilterEventProcessor<FilterRegistrationRequest, FilterResponse>
    {
        readonly ScopeId _scopeId;
        readonly FilterEventCallback _filterEventCallback;
        readonly FiltersClient _client;
        readonly ICreateReverseCallClients _reverseCallClientsCreator;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventFilterProcessor"/> class.
        /// </summary>
        /// <param name="filterId">The <see cref="FilterId" />.</param>
        /// <param name="scopeId">The <see cref="ScopeId" />.</param>
        /// <param name="filterEventCallback">The <see cref="FilterEventCallback" />.</param>
        /// <param name="client">The <see cref="FiltersClient" />.</param>
        /// <param name="reverseCallClientsCreator">The <see cref="ICreateReverseCallClients" />.</param>
        /// <param name="eventTypes">The <see cref="EventTypes" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EventFilterProcessor(
            FilterId filterId,
            ScopeId scopeId,
            FilterEventCallback filterEventCallback,
            FiltersClient client,
            ICreateReverseCallClients reverseCallClientsCreator,
            EventTypes eventTypes,
            ILogger<EventFilterProcessor> logger)
            : base("Filter", filterId, eventTypes, logger)
        {
            _scopeId = scopeId;
            _filterEventCallback = filterEventCallback;
            _client = client;
            _reverseCallClientsCreator = reverseCallClientsCreator;
        }

        /// <inheritdoc/>
        protected override FilterRegistrationRequest RegisterArguments
            => new FilterRegistrationRequest
                {
                    FilterId = Identifier.ToProtobuf(),
                    ScopeId = _scopeId.ToProtobuf()
                };

        /// <inheritdoc/>
        protected override IReverseCallClient<FilterRegistrationRequest, FilterRegistrationResponse, FilterEventRequest, FilterResponse> CreateClient(
            FilterRegistrationRequest registerArguments,
            Func<FilterEventRequest, CancellationToken, Task<FilterResponse>> callback,
            uint pingTimeout,
            CancellationToken cancellation)
            => _reverseCallClientsCreator.Create(
                RegisterArguments,
                this,
                new DuplexStreamingMethodCaller(_client),
                new ReverseCallMessageConverter());

        /// <inheritdoc/>
        protected override FilterResponse CreateResponseFromFailure(ProcessorFailure failure)
            => new FilterResponse { Failure = failure };

        /// <inheritdoc/>
        protected override async Task<FilterResponse> Filter(object @event, EventContext context)
        {
            var shouldInclude = await _filterEventCallback(@event, context).ConfigureAwait(false);
            var response = new FilterResponse { IsIncluded = shouldInclude };

            return response;
        }

        class DuplexStreamingMethodCaller : ICanCallADuplexStreamingMethod<FiltersClient, FilterClientToRuntimeMessage, FilterRuntimeToClientMessage>
        {
            readonly FiltersClient _client;

            public DuplexStreamingMethodCaller(FiltersClient client)
            {
                _client = client;
            }

            public AsyncDuplexStreamingCall<FilterClientToRuntimeMessage, FilterRuntimeToClientMessage> Call(Channel channel, CallOptions callOptions)
                => _client.Connect(callOptions);
        }

        class ReverseCallMessageConverter : IConvertReverseCallMessages<FilterClientToRuntimeMessage, FilterRuntimeToClientMessage, FilterRegistrationRequest, FilterRegistrationResponse, FilterEventRequest, FilterResponse>
        {
            public FilterClientToRuntimeMessage CreateMessageFrom(FilterRegistrationRequest arguments)
                => new FilterClientToRuntimeMessage { RegistrationRequest = arguments };

            public FilterClientToRuntimeMessage CreateMessageFrom(Pong pong)
                => new FilterClientToRuntimeMessage { Pong = pong };

            public FilterClientToRuntimeMessage CreateMessageFrom(FilterResponse response)
                => new FilterClientToRuntimeMessage { FilterResult = response };

            public FilterRegistrationResponse GetConnectResponseFrom(FilterRuntimeToClientMessage message)
                => message.RegistrationResponse;

            public Ping GetPingFrom(FilterRuntimeToClientMessage message)
                => message.Ping;

            public ReverseCallRequestContext GetRequestContextFrom(FilterEventRequest message)
                => message.CallContext;

            public FilterEventRequest GetRequestFrom(FilterRuntimeToClientMessage message)
                => message.FilterRequest;

            public void SetConnectArgumentsContextIn(ReverseCallArgumentsContext context, FilterRegistrationRequest arguments)
                => arguments.CallContext = context;

            public void SetResponseContextIn(ReverseCallResponseContext context, FilterResponse response)
                => response.CallContext = context;
        }
    }
}
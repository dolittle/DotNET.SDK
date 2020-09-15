// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.SDK.Events.Processing;
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
        readonly ICreateReverseCallClients _reverseCallClientsCreator;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventFilterProcessor"/> class.
        /// </summary>
        /// <param name="filterId">The <see cref="FilterId" />.</param>
        /// <param name="scopeId">The <see cref="ScopeId" />.</param>
        /// <param name="filterEventCallback">The <see cref="FilterEventCallback" />.</param>
        /// <param name="reverseCallClientsCreator">The <see cref="ICreateReverseCallClients" />.</param>
        /// <param name="processingRequestConverter">The <see cref="IEventProcessingRequestConverter" />.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
        public EventFilterProcessor(
            FilterId filterId,
            ScopeId scopeId,
            FilterEventCallback filterEventCallback,
            ICreateReverseCallClients reverseCallClientsCreator,
            IEventProcessingRequestConverter processingRequestConverter,
            ILoggerFactory loggerFactory)
            : base(
                "Filter",
                filterId,
                reverseCallClientsCreator.Create(
                    new FilterRegistrationRequest
                    {
                        FilterId = filterId.ToProtobuf(),
                        ScopeId = scopeId.ToProtobuf()
                    },
                    this,
                    new EventFilterProtocol()),
                processingRequestConverter,
                loggerFactory)
        {
            _scopeId = scopeId;
            _filterEventCallback = filterEventCallback;
            _reverseCallClientsCreator = reverseCallClientsCreator;
        }

        /// <inheritdoc/>
        protected override FilterResponse CreateResponseFromFailure(ProcessorFailure failure)
            => new FilterResponse { Failure = failure };

        /// <inheritdoc/>
        protected override async Task<FilterResponse> Filter(object @event, EventContext context)
            => new FilterResponse { IsIncluded = await _filterEventCallback(@event, context).ConfigureAwait(false) };
    }
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.SDK.Protobuf;
using Dolittle.SDK.Services;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Filters.Internal
{
    /// <summary>
    /// An implementation of <see cref="FilterEventProcessorRegistration{TClientMessage, TRegisterArguments, TResponse}" /> for private non-partitioned event filter.
    /// </summary>
    public class EventFilterProcessorRegistration : FilterEventProcessorRegistration<FilterClientToRuntimeMessage, FilterRegistrationRequest, FilterResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventFilterProcessorRegistration"/> class.
        /// </summary>
        /// <param name="filterId">The <see cref="FilterId" />.</param>
        /// <param name="scopeId">The <see cref="ScopeId" />.</param>
        /// <param name="reverseCallHandler">The <see cref="IReverseCallHandler{TRequest, TResponse}" />.</param>
        /// <param name="reverseCallClientsCreator">The <see cref="ICreateReverseCallClients" />.</param>
        /// <param name="protocol">The <see cref="IAmAFilterProtocol{TClientMessage, TRegisterArguments, TResponse}" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EventFilterProcessorRegistration(
            FilterId filterId,
            ScopeId scopeId,
            IReverseCallHandler<FilterEventRequest, FilterResponse> reverseCallHandler,
            ICreateReverseCallClients reverseCallClientsCreator,
            IAmAFilterProtocol<FilterClientToRuntimeMessage, FilterRegistrationRequest, FilterResponse> protocol,
            ILogger logger)
            : base(filterId, EventFilterProcessor.Kind, reverseCallHandler, reverseCallClientsCreator, protocol, logger)
            => Arguments = new FilterRegistrationRequest
                {
                    FilterId = filterId.ToProtobuf(),
                    ScopeId = scopeId.ToProtobuf()
                };

        /// <inheritdoc/>
        protected override FilterRegistrationRequest Arguments { get; }
    }
}
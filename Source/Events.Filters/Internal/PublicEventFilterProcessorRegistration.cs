// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.SDK.Protobuf;
using Dolittle.SDK.Services;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Filters.Internal
{
    /// <summary>
    /// An implementation of <see cref="FilterEventProcessorRegistration{TClientMessage, TRegisterArguments, TResponse}" /> for public event filter.
    /// </summary>
    public class PublicEventFilterProcessorRegistration : FilterEventProcessorRegistration<PublicFilterClientToRuntimeMessage, PublicFilterRegistrationRequest, PartitionedFilterResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PublicEventFilterProcessorRegistration"/> class.
        /// </summary>
        /// <param name="filterId">The <see cref="FilterId" />.</param>
        /// <param name="reverseCallHandler">The <see cref="IReverseCallHandler{TRequest, TResponse}" />.</param>
        /// <param name="reverseCallClientsCreator">The <see cref="ICreateReverseCallClients" />.</param>
        /// <param name="protocol">The <see cref="IAmAFilterProtocol{TClientMessage, TRegisterArguments, TResponse}" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public PublicEventFilterProcessorRegistration(
            FilterId filterId,
            IReverseCallHandler<FilterEventRequest, PartitionedFilterResponse> reverseCallHandler,
            ICreateReverseCallClients reverseCallClientsCreator,
            IAmAFilterProtocol<PublicFilterClientToRuntimeMessage, PublicFilterRegistrationRequest, PartitionedFilterResponse> protocol,
            ILogger logger)
            : base(filterId, PartitionedEventFilterProcessor.Kind, reverseCallHandler, reverseCallClientsCreator, protocol, logger)
            => Arguments = new PublicFilterRegistrationRequest
                {
                    FilterId = filterId.ToProtobuf()
                };

        /// <inheritdoc/>
        protected override PublicFilterRegistrationRequest Arguments { get; }
    }
}
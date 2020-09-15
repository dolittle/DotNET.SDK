// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Protobuf.Contracts;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.SDK.Events.Processing;
using Dolittle.SDK.Events.Processing.Internal;
using Dolittle.SDK.Services;
using Google.Protobuf;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Filters.Internal
{
    /// <summary>
    /// An abstract implementation of <see cref="EventProcessorRegistration{TIdentifier, TClientMessage, TServerMessage, TRegisterArguments, TRegisterResponse, TRequest, TResponse}" /> for event filters.
    /// </summary>
    /// <typeparam name="TClientMessage">The <see cref="System.Type" /> of the client message.</typeparam>
    /// <typeparam name="TRegisterArguments">The <see cref="System.Type" /> of the registration arguments.</typeparam>
    /// <typeparam name="TResponse">The <see cref="System.Type" /> of the response.</typeparam>
    public abstract class FilterEventProcessorRegistration<TClientMessage, TRegisterArguments, TResponse> : EventProcessorRegistration<FilterId, TClientMessage, FilterRuntimeToClientMessage, TRegisterArguments, FilterRegistrationResponse, FilterEventRequest, TResponse>
        where TClientMessage : IMessage
        where TRegisterArguments : class
        where TResponse : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterEventProcessorRegistration{TClientMessage, TRegisterArguments, TResponse}"/> class.
        /// </summary>
        /// <param name="filterId">The <see cref="FilterId" />.</param>
        /// <param name="eventProcessorKind">The <see cref="EventProcessorKind" />.</param>
        /// <param name="reverseCallHandler">The <see cref="IReverseCallHandler{TRequest, TResponse}" />.</param>
        /// <param name="reverseCallClientsCreator">The <see cref="ICreateReverseCallClients" />.</param>
        /// <param name="protocol">The <see cref="IAmAReverseCallProtocol{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        protected FilterEventProcessorRegistration(
            FilterId filterId,
            EventProcessorKind eventProcessorKind,
            IReverseCallHandler<FilterEventRequest, TResponse> reverseCallHandler,
            ICreateReverseCallClients reverseCallClientsCreator,
            IAmAFilterProtocol<TClientMessage, TRegisterArguments, TResponse> protocol,
            ILogger logger)
            : base(reverseCallHandler, reverseCallClientsCreator, protocol, filterId, eventProcessorKind, logger)
        {
        }

        /// <inheritdoc/>
        protected override Failure GetFailureFromRegisterResponse(FilterRegistrationResponse response)
            => response.Failure;
    }
}
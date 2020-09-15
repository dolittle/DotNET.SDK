// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.Protobuf.Contracts;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.SDK.Events.Processing;
using Dolittle.SDK.Events.Processing.Internal;
using Dolittle.SDK.Protobuf;
using Dolittle.SDK.Services;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Handling.Internal
{
    /// <summary>
    /// Represents a <see cref="EventProcessor{TIdentifier, TRegisterArguments, TRegisterResponse, TRequest, TResponse}" /> that can handle events.
    /// </summary>
    public class EventHandlerRegistration : EventProcessorRegistration<EventHandlerId, EventHandlerClientToRuntimeMessage, EventHandlerRuntimeToClientMessage, EventHandlerRegistrationRequest, EventHandlerRegistrationResponse, HandleEventRequest, EventHandlerResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlerRegistration"/> class.
        /// </summary>
        /// <param name="eventHandlerId">The <see cref="EventHandlerId" />.</param>
        /// <param name="eventProcessorKind">The <see cref="EventProcessorKind" />.</param>
        /// <param name="scopeId">The <see cref="ScopeId" />.</param>
        /// <param name="partitioned">Wether the event handler is partitioned or not.</param>
        /// <param name="handledEvents">The <see cref="IEnumerable{T}" /> of <see cref="EventType" /> that the <see cref="IEventHandler" /> handles.</param>
        /// <param name="reverseCallHandler">The <see cref="IReverseCallHandler{TRequest, TResponse}" />.</param>
        /// <param name="reverseCallClientsCreator">The <see cref="ICreateReverseCallClients" />.</param>
        /// <param name="protocol">The <see cref="IAmAReverseCallProtocol{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EventHandlerRegistration(
            EventHandlerId eventHandlerId,
            EventProcessorKind eventProcessorKind,
            ScopeId scopeId,
            bool partitioned,
            IEnumerable<EventType> handledEvents,
            IReverseCallHandler<HandleEventRequest, EventHandlerResponse> reverseCallHandler,
            ICreateReverseCallClients reverseCallClientsCreator,
            IAmAReverseCallProtocol<EventHandlerClientToRuntimeMessage, EventHandlerRuntimeToClientMessage, EventHandlerRegistrationRequest, EventHandlerRegistrationResponse, HandleEventRequest, EventHandlerResponse> protocol,
            ILogger logger)
            : base(reverseCallHandler, reverseCallClientsCreator, protocol, eventHandlerId, eventProcessorKind, logger)
            {
                Arguments = new EventHandlerRegistrationRequest
                    {
                        EventHandlerId = eventHandlerId.ToProtobuf(),
                        ScopeId = scopeId.ToProtobuf(),
                        Partitioned = partitioned
                    };
                Arguments.Types_.AddRange(handledEvents.Select(_ => _.ToProtobuf()).ToArray());
            }

        /// <inheritdoc/>
        protected override EventHandlerRegistrationRequest Arguments { get; }

        /// <inheritdoc/>
        protected override Failure GetFailureFromRegisterResponse(EventHandlerRegistrationResponse response)
            => response.Failure;
    }
}
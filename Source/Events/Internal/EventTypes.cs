// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Protobuf;
using Dolittle.SDK.Services;
using Dolittle.Services.Contracts;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.Events.Internal
{
    /// <summary>
    /// Represents a system that knows how to register event types with the Runtime.
    /// </summary>
    public class EventTypes
    {
        static readonly EventTypesRegisterMethod _method = new EventTypesRegisterMethod();
        readonly IPerformMethodCalls _caller;
        readonly ExecutionContext _executionContext;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventTypes"/> class.
        /// </summary>
        /// <param name="caller">The method caller to use to perform calls to the Runtime.</param>
        /// <param name="executionContext">Tha base <see cref="ExecutionContext"/>.</param>
        /// <param name="logger">The <see cref="ILogger"/> to use.</param>
        public EventTypes(IPerformMethodCalls caller, ExecutionContext executionContext, ILogger logger)
        {
            _caller = caller;
            _executionContext = executionContext;
            _logger = logger;
        }

        /// <summary>
        /// Registers event types.
        /// </summary>
        /// <param name="eventTypes">The event types to register.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task Register(IEventTypes eventTypes, CancellationToken cancellationToken)
            => Task.WhenAll(eventTypes.All.Select(CreateRequest).Select(_ => Register(_, cancellationToken)));

        EventTypeRegistrationRequest CreateRequest(EventType eventType)
            => new EventTypeRegistrationRequest
            {
                Alias = eventType.HasAlias ? eventType.Alias : null,
                EventType = eventType.ToProtobuf(),
                CallContext = new CallRequestContext
                {
                    ExecutionContext = _executionContext.ToProtobuf(),
                    HeadId = HeadId.NotSet.ToProtobuf()
                }
            };

        async Task Register(EventTypeRegistrationRequest request, CancellationToken cancellationToken)
        {
            _logger.LogDebug(
                "Registering Event Type {EventType} with Alias {Alias}",
                request.EventType.Id.ToGuid(),
                request.Alias);
            try
            {
                var response = await _caller.Call(_method, request, cancellationToken).ConfigureAwait(false);
                if (response.Failure != null)
                {
                    _logger.LogWarning(
                        "An error occurred while registering Event Type {EventType} with Alias {Alias} because {Reason}",
                        request.EventType.Id.ToGuid(),
                        request.Alias,
                        response.Failure.Reason);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "An error occurred while registering Event Type {EventType} with Alias {Alias}",
                    request.EventType.Id.ToGuid(),
                    request.Alias);
            }
        }
    }
}
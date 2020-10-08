// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Execution;
using Dolittle.SDK.Services;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Represents a builder for building <see cref="EventStore"/>.
    /// </summary>
    public class EventStoreBuilder
    {
        readonly IPerformMethodCalls _caller;
        readonly ExecutionContext _executionContext;
        readonly IEventTypes _eventTypes;
        readonly IConvertEventsToProtobuf _eventsToProtobufConverter;
        readonly IConvertAggregateEventsToProtobuf _aggregateEventsToProtobufConverter;
        readonly IConvertEventResponsestoSDK _eventResponsestoSDKConverter;
        readonly IConvertAggregateResponsesToSDK _aggregateResponsesToSDKConverter;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStoreBuilder"/> class.
        /// </summary>
        /// <param name="caller">The caller for unary calls.</param>
        /// <param name="executionContext">The base <see cref="ExecutionContext"/> to use.</param>
        /// <param name="eventTypes">The <see cref="IEventTypes"/>.</param>
        /// <param name="convertEventsToProtobuf">The <see cref="IConvertEventsToProtobuf"/>.</param>
        /// <param name="convertAggregateEventsToProtobuf">The <see cref="IConvertAggregateEventsToProtobuf"/>.</param>
        /// <param name="convertEventResponsestoSDK">The <see cref="IConvertEventResponsestoSDK"/>.</param>
        /// <param name="convertAggregateResponsesToSDK">The <see cref="IConvertAggregateResponsesToSDK"/>.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EventStoreBuilder(
            IPerformMethodCalls caller,
            ExecutionContext executionContext,
            IEventTypes eventTypes,
            IConvertEventsToProtobuf convertEventsToProtobuf,
            IConvertAggregateEventsToProtobuf convertAggregateEventsToProtobuf,
            IConvertEventResponsestoSDK convertEventResponsestoSDK,
            IConvertAggregateResponsesToSDK convertAggregateResponsesToSDK,
            ILogger logger)
        {
            _caller = caller;
            _executionContext = executionContext;
            _eventTypes = eventTypes;
            _eventsToProtobufConverter = convertEventsToProtobuf;
            _aggregateEventsToProtobufConverter = convertAggregateEventsToProtobuf;
            _eventResponsestoSDKConverter = convertEventResponsestoSDK;
            _aggregateResponsesToSDKConverter = convertAggregateResponsesToSDK;
            _logger = logger;
        }

        /// <summary>
        /// Creates an <see cref="IEventStore"/> for the given tenant.
        /// </summary>
        /// <param name="tenant">The <see cref="TenantId">tenant</see> to create the event store for.</param>
        /// <returns>An <see cref="IEventStore"/>.</returns>
        public IEventStore ForTenant(TenantId tenant)
            => new EventStore(
                _caller,
                _executionContext.ForTenant(tenant),
                _eventTypes,
                _eventsToProtobufConverter,
                _aggregateEventsToProtobufConverter,
                _eventResponsestoSDKConverter,
                _aggregateResponsesToSDKConverter,
                _logger);
    }
}

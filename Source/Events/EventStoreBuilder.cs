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
        readonly IEventConverter _eventConverter;
        readonly ExecutionContext _executionContext;
        readonly IEventTypes _eventTypes;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStoreBuilder"/> class.
        /// </summary>
        /// <param name="caller">The caller for unary calls.</param>
        /// <param name="eventConverter">The <see cref="IEventConverter" />.</param>
        /// <param name="executionContext">The base <see cref="ExecutionContext"/> to use.</param>
        /// <param name="eventTypes">The <see cref="IEventTypes"/>.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EventStoreBuilder(
            IPerformMethodCalls caller,
            IEventConverter eventConverter,
            ExecutionContext executionContext,
            IEventTypes eventTypes,
            ILogger logger)
        {
            _caller = caller;
            _eventConverter = eventConverter;
            _executionContext = executionContext;
            _eventTypes = eventTypes;
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
                _eventConverter,
                _executionContext.ForTenant(tenant),
                _eventTypes,
                _logger);
    }
}

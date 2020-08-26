// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Events.Filters.EventHorizon;
using Dolittle.Logging;
using Dolittle.Services.Clients;
using static Dolittle.Runtime.Events.Processing.Contracts.Filters;

namespace Dolittle.Events.Filters.Internal
{
    /// <summary>
    /// A class used to construct instances of <see cref="AbstractFilterProcessor{TEventType, TClientMessage, TRegistrationRequest, TFilterResponse}"/>.
    /// </summary>
    public class FilterProcessors
    {
        readonly FiltersClient _client;
        readonly IReverseCallClients _reverseCallClients;
        readonly IEventConverter _converter;
        readonly ILoggerManager _loggerManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterProcessors"/> class.
        /// </summary>
        /// <param name="client">The <see cref="FiltersClient"/> to use to connect to the Runtime.</param>
        /// <param name="reverseCallClients">The <see cref="IReverseCallClients"/> to use for creating instances of <see cref="IReverseCallClient{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}"/>.</param>
        /// <param name="converter">The <see cref="IEventConverter"/> to use to convert events.</param>
        /// <param name="loggerManager">The <see cref="ILoggerManager"/> to use for creating instances of <see cref="ILogger"/>.</param>
        public FilterProcessors(
            FiltersClient client,
            IReverseCallClients reverseCallClients,
            IEventConverter converter,
            ILoggerManager loggerManager)
        {
            _client = client;
            _reverseCallClients = reverseCallClients;
            _converter = converter;
            _loggerManager = loggerManager;
        }

        /// <summary>
        /// Creates an <see cref="EventFilterProcessor"/> for registering and invoking an instance of <see cref="ICanFilterEvents"/>.
        /// </summary>
        /// <param name="id">The unique <see cref="FilterId"/> for the event filter.</param>
        /// <param name="scope">The <see cref="ScopeId"/> of the scope in the Event Store where the filter will run.</param>
        /// <param name="filter">The <see cref="ICanFilterEvents"/> to use for filtering the events.</param>
        /// <returns>An <see cref="EventFilterProcessor"/> for registering and invoking an instance of <see cref="ICanFilterEvents"/>.</returns>
        public EventFilterProcessor GetFor(FilterId id, ScopeId scope, ICanFilterEvents filter)
            => new EventFilterProcessor(id, scope, _client, _reverseCallClients, filter, _converter, _loggerManager.CreateLogger<EventFilterProcessor>());

        /// <summary>
        /// Creates an <see cref="EventFilterWithPartitionsProcessor"/> for registering and invoking an instance of <see cref="ICanFilterEventsWithPartition"/>.
        /// </summary>
        /// <param name="id">The unique <see cref="FilterId"/> for the event filter.</param>
        /// <param name="scope">The <see cref="ScopeId"/> of the scope in the Event Store where the filter will run.</param>
        /// <param name="filter">The <see cref="ICanFilterEventsWithPartition"/> to use for filtering the events.</param>
        /// <returns>An <see cref="EventFilterProcessor"/> for registering and invoking an instance of <see cref="ICanFilterEventsWithPartition"/>.</returns>
        public EventFilterWithPartitionsProcessor GetFor(FilterId id, ScopeId scope, ICanFilterEventsWithPartition filter)
            => new EventFilterWithPartitionsProcessor(id, scope, _client, _reverseCallClients, filter, _converter, _loggerManager.CreateLogger<EventFilterWithPartitionsProcessor>());

        /// <summary>
        /// Creates a <see cref="PublicEventFilterProcessor"/> for registering and invoking an instance of <see cref="ICanFilterPublicEvents"/>.
        /// </summary>
        /// <param name="id">The unique <see cref="FilterId"/> for the event filter.</param>
        /// <param name="filter">The <see cref="ICanFilterPublicEvents"/> to use for filtering the events.</param>
        /// <returns>A <see cref="PublicEventFilterProcessor"/> for registering and invoking an instance of <see cref="ICanFilterPublicEvents"/>.</returns>
        public PublicEventFilterProcessor GetFor(FilterId id, ICanFilterPublicEvents filter)
            => new PublicEventFilterProcessor(id, _client, _reverseCallClients, filter, _converter, _loggerManager.CreateLogger<PublicEventFilterProcessor>());
    }
}

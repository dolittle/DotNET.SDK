// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Services;
using Microsoft.Extensions.Logging;
using Contracts = Dolittle.Runtime.Events.Contracts;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.Events.Store.Internal
{
    /// <summary>
    /// Represents an implementation of <see cref="ICommitAggregateEvents" />.
    /// </summary>
    public class AggregateEventCommitter : ICommitAggregateEvents
    {
        static readonly EventStoreCommitForAggregateMethod _commitForAggregateMethod = new EventStoreCommitForAggregateMethod();
        readonly IPerformMethodCalls _caller;
        readonly IEventConverter _eventConverter;
        readonly IResolveCallContext _callContextResolver;
        readonly ExecutionContext _executionContext;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateEventCommitter"/> class.
        /// </summary>
        /// <param name="caller">The <see cref="IPerformMethodCalls" />.</param>
        /// <param name="eventConverter">The <see cref="IEventConverter" />.</param>
        /// <param name="callContextResolver">The <see cref="IResolveCallContext" />.</param>
        /// <param name="executionContext">The <see cref="ExecutionContext" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public AggregateEventCommitter(
            IPerformMethodCalls caller,
            IEventConverter eventConverter,
            IResolveCallContext callContextResolver,
            ExecutionContext executionContext,
            ILogger logger)
        {
            _caller = caller;
            _eventConverter = eventConverter;
            _callContextResolver = callContextResolver;
            _executionContext = executionContext;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<CommitEventsForAggregateResult> CommitForAggregate(UncommittedAggregateEvents uncommittedAggregateEvents, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug(
                "Committing events for aggregate root {AggregateRoot} with expected version {ExpectedVersion}",
                uncommittedAggregateEvents.AggregateRootId,
                uncommittedAggregateEvents.ExpectedAggregateRootVersion);

            var request = new Contracts.CommitAggregateEventsRequest
            {
                CallContext = _callContextResolver.ResolveFrom(_executionContext),
                Events = _eventConverter.ToProtobuf(uncommittedAggregateEvents)
            };
            var response = await _caller.Call(_commitForAggregateMethod, request, cancellationToken).ConfigureAwait(false);
            return _eventConverter.ToSDK(response);
        }
    }
}

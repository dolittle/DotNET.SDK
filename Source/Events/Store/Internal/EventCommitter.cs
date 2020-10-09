// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Failures;
using Dolittle.SDK.Services;
using Microsoft.Extensions.Logging;
using Contracts = Dolittle.Runtime.Events.Contracts;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.Events.Store.Internal
{
    /// <summary>
    /// Represents an implementation of <see cref="ICommitEvents" />.
    /// </summary>
    public class EventCommitter : ICommitEvents
    {
        static readonly EventStoreCommitMethod _commitForAggregateMethod = new EventStoreCommitMethod();
        readonly IPerformMethodCalls _caller;
        readonly IEventConverter _eventConverter;
        readonly IResolveCallContext _callContextResolver;
        readonly ExecutionContext _executionContext;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventCommitter"/> class.
        /// </summary>
        /// <param name="caller">The <see cref="IPerformMethodCalls" />.</param>
        /// <param name="eventConverter">The <see cref="IEventConverter" />.</param>
        /// <param name="callContextResolver">The <see cref="IResolveCallContext" />.</param>
        /// <param name="executionContext">The <see cref="ExecutionContext" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EventCommitter(
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
        public async Task<CommittedEvents> Commit(UncommittedEvents uncommittedEvents, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Committing events");

            var request = new Contracts.CommitEventsRequest
            {
                CallContext = _callContextResolver.ResolveFrom(_executionContext)
            };
            request.Events.AddRange(_eventConverter.ToProtobuf(uncommittedEvents));
            var response = await _caller.Call(_commitForAggregateMethod, request, cancellationToken).ConfigureAwait(false);
            response.Failure.ThrowIfFailureIsSet();
            return _eventConverter.ToSDK(response.Events);
        }
    }
}

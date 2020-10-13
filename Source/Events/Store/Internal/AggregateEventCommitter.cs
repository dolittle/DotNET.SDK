// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Events.Store.Converters;
using Dolittle.SDK.Failures;
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
        readonly IConvertAggregateEventsToProtobuf _toProtobuf;
        readonly IConvertAggregateEventsToSDK _toSDK;
        readonly IResolveCallContext _callContextResolver;
        readonly ExecutionContext _executionContext;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateEventCommitter"/> class.
        /// </summary>
        /// <param name="caller">The <see cref="IPerformMethodCalls" />.</param>
        /// <param name="toProtobuf">The <see cref="IConvertAggregateEventsToProtobuf" />.</param>
        /// <param name="toSDK">The <see cref="IConvertAggregateEventsToSDK" />.</param>
        /// <param name="callContextResolver">The <see cref="IResolveCallContext" />.</param>
        /// <param name="executionContext">The <see cref="ExecutionContext" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public AggregateEventCommitter(
            IPerformMethodCalls caller,
            IConvertAggregateEventsToProtobuf toProtobuf,
            IConvertAggregateEventsToSDK toSDK,
            IResolveCallContext callContextResolver,
            ExecutionContext executionContext,
            ILogger logger)
        {
            _caller = caller;
            _toProtobuf = toProtobuf;
            _toSDK = toSDK;
            _callContextResolver = callContextResolver;
            _executionContext = executionContext;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<CommittedAggregateEvents> CommitForAggregate(UncommittedAggregateEvents uncommittedAggregateEvents, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug(
                "Committing {NumberOfEvents} events for aggregate root type {AggregateRoot} and id {EventSource} with expected version {ExpectedVersion}",
                uncommittedAggregateEvents.Count,
                uncommittedAggregateEvents.AggregateRoot,
                uncommittedAggregateEvents.EventSource,
                uncommittedAggregateEvents.ExpectedAggregateRootVersion);

            if (!_toProtobuf.TryConvert(uncommittedAggregateEvents, out var protobufEvents, out var error))
            {
                _logger.LogError(error, "Could not convert {UncommittedAggregateEvents} to Protobuf.", uncommittedAggregateEvents);
                throw error;
            }

            var request = new Contracts.CommitAggregateEventsRequest
            {
                CallContext = _callContextResolver.ResolveFrom(_executionContext),
                Events = protobufEvents
            };
            var response = await _caller.Call(_commitForAggregateMethod, request, cancellationToken).ConfigureAwait(false);
            response.Failure.ThrowIfFailureIsSet();

            if (!_toSDK.TryConvert(response.Events, out var committedAggregateEvents, out error))
            {
                _logger.LogError(error, "The Runtime acknowledges that the events have been committed, but the returned {CommittedAggregateEvents} could not be converted.", response.Events);
                throw error;
            }

            return committedAggregateEvents;
        }
    }
}

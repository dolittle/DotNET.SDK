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
    /// Represents an implementation of <see cref="ICommitEvents" />.
    /// </summary>
    public class EventCommitter : ICommitEvents
    {
        static readonly EventStoreCommitMethod _commitForAggregateMethod = new EventStoreCommitMethod();
        readonly IPerformMethodCalls _caller;
        readonly IConvertEventsToProtobuf _toProtobuf;
        readonly IConvertEventsToSDK _toSDK;
        readonly IResolveCallContext _callContextResolver;
        readonly ExecutionContext _executionContext;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventCommitter"/> class.
        /// </summary>
        /// <param name="caller">The <see cref="IPerformMethodCalls" />.</param>
        /// <param name="toProtobuf">The <see cref="IConvertEventsToProtobuf" />.</param>
        /// <param name="toSDK">The <see cref="IConvertEventsToSDK" />.</param>
        /// <param name="callContextResolver">The <see cref="IResolveCallContext" />.</param>
        /// <param name="executionContext">The <see cref="ExecutionContext" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EventCommitter(
            IPerformMethodCalls caller,
            IConvertEventsToProtobuf toProtobuf,
            IConvertEventsToSDK toSDK,
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
        public async Task<CommittedEvents> Commit(UncommittedEvents uncommittedEvents, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Committing {NumberOfEvents} events", uncommittedEvents.Count);

            if (!_toProtobuf.TryConvert(uncommittedEvents, out var protobufEvents, out var error))
            {
                _logger.LogError(error, "Could not convert {UncommittedEvents} to Protobuf.", uncommittedEvents);
                throw error;
            }

            var request = new Contracts.CommitEventsRequest
            {
                CallContext = _callContextResolver.ResolveFrom(_executionContext)
            };
            request.Events.AddRange(protobufEvents);

            var response = await _caller.Call(_commitForAggregateMethod, request, cancellationToken).ConfigureAwait(false);
            response.Failure.ThrowIfFailureIsSet();

            if (!_toSDK.TryConvert(response.Events, out var committedEvents, out error))
            {
                _logger.LogError(error, "Could not convert {CommittedEvents} to SDK. The events have been committed.", response.Events);
                throw error;
            }

            return committedEvents;
        }
    }
}

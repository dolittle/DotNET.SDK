// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Diagnostics;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.SDK.Events.Store.Converters;
using Dolittle.SDK.Failures;
using Dolittle.SDK.Services;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.Events.Store.Internal;

/// <summary>
/// Represents an implementation of <see cref="ICommitAggregateEvents" />.
/// </summary>
public class AggregateEventCommitter : ICommitAggregateEvents
{
    static readonly EventStoreCommitForAggregateMethod _commitForAggregateMethod = new();
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
        using var activity = Tracing.ActivitySource.StartActivity()
            ?.Tag(uncommittedAggregateEvents);

        try
        {
            _logger.CommittingAggregateEvents(uncommittedAggregateEvents.Count,
                uncommittedAggregateEvents.AggregateRoot,
                uncommittedAggregateEvents.EventSource,
                uncommittedAggregateEvents.ExpectedAggregateRootVersion);

            if (!_toProtobuf.TryConvert(uncommittedAggregateEvents, out var protobufEvents, out var error))
            {
                _logger.UncommittedAggregateEventsCouldNotBeConverted(error);
                throw error;
            }

            var request = new CommitAggregateEventsRequest
            {
                CallContext = _callContextResolver.ResolveFrom(_executionContext),
                Events = protobufEvents
            };
            var response = await _caller.Call(_commitForAggregateMethod, request, cancellationToken).ConfigureAwait(false);
            response.Failure.ThrowIfFailureIsSet();

            if (_toSDK.TryConvert(response.Events, out var committedAggregateEvents, out error))
            {
                return committedAggregateEvents;
            }
            _logger.CommittedAggregateEventsCouldNotBeConverted(error);
            throw error;
        }
        catch (Exception e)
        {
            activity?.RecordError(e);
            throw;
        }

    }
}

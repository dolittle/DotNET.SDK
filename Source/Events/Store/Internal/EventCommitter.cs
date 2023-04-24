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
/// Represents an implementation of <see cref="ICommitEvents" />.
/// </summary>
public class EventCommitter : ICommitEvents
{
    static readonly EventStoreCommitMethod _commitForAggregateMethod = new();
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
        using var activity = Tracing.ActivitySource.StartActivity()
            ?.Tag(uncommittedEvents);

        try
        {
            _logger.CommittingEvents(uncommittedEvents.Count, _executionContext.Tenant);

            if (!_toProtobuf.TryConvert(uncommittedEvents, out var protobufEvents, out var error))
            {
                _logger.UncommittedEventsCouldNotBeConverted(error);
                throw error;
            }

            var request = new CommitEventsRequest
            {
                CallContext = _callContextResolver.ResolveFrom(_executionContext)
            };
            request.Events.AddRange(protobufEvents);

            var response = await _caller.Call(_commitForAggregateMethod, request, cancellationToken).ConfigureAwait(false);
            response.Failure.ThrowIfFailureIsSet();

            if (_toSDK.TryConvert(response.Events, out var committedEvents, out error))
            {
                return committedEvents;
            }
            _logger.CommittedEventsCouldNotBeConverted(error);
            throw error;
        }
        catch (Exception e)
        {
            activity?.RecordError(e);
            throw;
        }
    }
}

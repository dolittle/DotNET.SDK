// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.SDK.Events.Filters.Internal;
using Dolittle.SDK.Events.Processing;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Filters.Builders.Partitioned;

/// <summary>
/// Represents an implementation of <see cref="ICanRegisterEventFilterProcessor"/> that can register a partitioned event filter.
/// </summary>
public class UnregisteredPartitionedEventFilter : ICanRegisterEventFilterProcessor
{
    static readonly PartitionedEventFilterProtocol _protocol = new();

    readonly FilterId _filterId;
    readonly ScopeId _scopeId;
    readonly PartitionedFilterEventCallback _callback;

    /// <summary>
    /// Initializes an instance of the <see cref="UnregisteredPartitionedEventFilter"/> class.
    /// </summary>
    /// <param name="filterId">The <see cref="FilterId"/>.</param>
    /// <param name="scopeId">The <see cref="ScopeId"/>.</param>
    /// <param name="callback">The <see cref="PartitionedFilterEventCallback"/>.</param>
    public UnregisteredPartitionedEventFilter(FilterId filterId, ScopeId scopeId, PartitionedFilterEventCallback callback)
    {
        _filterId = filterId;
        _scopeId = scopeId;
        _callback = callback;
    }

    /// <inheritdoc />
    public FilterModelId Identifier => new(_filterId, _scopeId); 

    /// <inheritdoc />
    public void Register(
        IEventProcessors eventProcessors,
        IEventProcessingConverter converter,
        ILoggerFactory loggerFactory,
        CancellationToken cancelConnectToken,
        CancellationToken stopProcessingToken)
    {
        var filter = new PartitionedEventFilterProcessor(_filterId, _scopeId, _callback, converter, loggerFactory);
        eventProcessors.Register(filter, _protocol, cancelConnectToken, stopProcessingToken);
    }
}

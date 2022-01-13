// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.SDK.Events.Filters.Internal;
using Dolittle.SDK.Events.Processing;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Filters.Builders.Partitioned.Public;

/// <summary>
/// Represents an implementation of <see cref="ICanRegisterEventFilterProcessor"/> that can register an public event filter.
/// </summary>
public class UnregisteredPublicEventFilter : ICanRegisterEventFilterProcessor
{
    static readonly PublicEventFilterProtocol _protocol = new();

    readonly FilterId _filterId;
    readonly PartitionedFilterEventCallback _callback;

    /// <summary>
    /// Initializes an instance of the <see cref="UnregisteredPublicEventFilter"/> class.
    /// </summary>
    /// <param name="filterId">The <see cref="FilterId"/>.</param>
    /// <param name="callback">The <see cref="PartitionedFilterEventCallback"/>.</param>
    public UnregisteredPublicEventFilter(FilterId filterId, PartitionedFilterEventCallback callback)
    {
        _filterId = filterId;
        _callback = callback;
    }

    /// <inheritdoc />
    public FilterModelId Identifier => new(_filterId, ScopeId.Default);

    /// <inheritdoc />
    public void Register(
        IEventProcessors eventProcessors,
        IEventProcessingConverter converter,
        ILoggerFactory loggerFactory,
        CancellationToken cancelConnectToken,
        CancellationToken stopProcessingToken)
    {
        var filter = new PublicEventFilterProcessor(_filterId, _callback, converter, loggerFactory);
        eventProcessors.Register(filter, _protocol, cancelConnectToken, stopProcessingToken);
    }
}

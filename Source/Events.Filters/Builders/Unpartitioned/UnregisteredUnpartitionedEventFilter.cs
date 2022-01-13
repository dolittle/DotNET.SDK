// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.SDK.Events.Filters.Internal;
using Dolittle.SDK.Events.Processing;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Filters.Builders.Unpartitioned;

/// <summary>
/// Represents an implementation of <see cref="ICanRegisterEventFilterProcessor"/> that can register an unpartitioned event filter.
/// </summary>
public class UnregisteredUnpartitionedEventFilter : ICanRegisterEventFilterProcessor
{
    static readonly UnpartitionedEventFilterProtocol _protocol = new();

    readonly FilterId _filterId;
    readonly ScopeId _scopeId;
    readonly FilterEventCallback _callback;

    /// <summary>
    /// Initializes an instance of the <see cref="UnregisteredUnpartitionedEventFilter"/> class.
    /// </summary>
    /// <param name="filterId">The <see cref="FilterId"/>.</param>
    /// <param name="scopeId">The <see cref="ScopeId"/>.</param>
    /// <param name="callback">The <see cref="FilterEventCallback"/>.</param>
    public UnregisteredUnpartitionedEventFilter(FilterId filterId, ScopeId scopeId, FilterEventCallback callback)
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
        var filter = new UnpartitionedEventFilterProcessor(_filterId, _scopeId, _callback, converter, loggerFactory);
        eventProcessors.Register(filter, _protocol, cancelConnectToken, stopProcessingToken);
    }
}

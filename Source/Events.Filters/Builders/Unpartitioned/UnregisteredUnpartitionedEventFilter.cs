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
    readonly FilterEventCallback _callback;

    /// <summary>
    /// Initializes an instance of the <see cref="UnregisteredUnpartitionedEventFilter"/> class.
    /// </summary>
    /// <param name="filterId">The <see cref="FilterModelId"/>.</param>
    /// <param name="scopeId">The <see cref="ScopeId"/>.</param>
    /// <param name="callback">The <see cref="FilterEventCallback"/>.</param>
    public UnregisteredUnpartitionedEventFilter(FilterModelId filterId, FilterEventCallback callback)
    {
        Identifier = filterId;
        _callback = callback;
    }

    /// <inheritdoc />
    public FilterModelId Identifier { get; }

    /// <inheritdoc />
    public void Register(
        IEventProcessors eventProcessors,
        IEventProcessingConverter converter,
        ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        var filter = new UnpartitionedEventFilterProcessor(Identifier, _callback, converter, loggerFactory);
        eventProcessors.Register(filter, _protocol, cancellationToken);
    }
}

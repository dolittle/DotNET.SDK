// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using Dolittle.SDK.Events.Processing;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Filters.Builders;

/// <summary>
/// Represents an implementation of <see cref="IUnregisteredEventFilters"/>.
/// </summary>
public class UnregisteredEventFilters : IUnregisteredEventFilters
{
    readonly IEnumerable<ICanRegisterEventFilterProcessor> _eventProcessors;

    /// <summary>
    /// Initializes an instance of the <see cref="UnregisteredEventFilters"/> class.
    /// </summary>
    /// <param name="eventProcessors">The <see cref="IEnumerable{T}"/> of <see cref="ICanRegisterEventFilterProcessor"/>.</param>
    public UnregisteredEventFilters(IEnumerable<ICanRegisterEventFilterProcessor> eventProcessors)
    {
        _eventProcessors = eventProcessors;
    }

    /// <inheritdoc />
    public void Register(IEventProcessors eventProcessors, IEventProcessingConverter processingConverter, ILoggerFactory loggerFactory, CancellationToken cancelConnectToken, CancellationToken stopProcessingToken)
    {
        foreach (var eventProcessor in _eventProcessors)
        {
            eventProcessor.Register(eventProcessors, processingConverter, loggerFactory, cancelConnectToken, stopProcessingToken);
        }
    }
}

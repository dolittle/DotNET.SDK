// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using Dolittle.SDK.Common;
using Dolittle.SDK.Events.Processing;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Filters.Builders;

/// <summary>
/// Represents an implementation of <see cref="IUnregisteredEventFilters"/>.
/// </summary>
public class UnregisteredEventFilters : UniqueBindings<FilterModelId, ICanRegisterEventFilterProcessor>, IUnregisteredEventFilters
{
    /// <summary>
    /// Initializes an instance of the <see cref="UnregisteredEventFilters"/> class.
    /// </summary>
    /// <param name="eventFilters">The <see cref="IEnumerable{T}"/> of <see cref="ICanRegisterEventFilterProcessor"/>.</param>
    public UnregisteredEventFilters(IUniqueBindings<FilterModelId, ICanRegisterEventFilterProcessor> eventFilters)
        : base(eventFilters)
    {
    }

    /// <inheritdoc />
    public void Register(IEventProcessors eventProcessors, IEventProcessingConverter processingConverter, ILoggerFactory loggerFactory, CancellationToken cancellationToken)
    {
        foreach (var eventFilter in Values)
        {
            eventFilter.Register(eventProcessors, processingConverter, loggerFactory, cancellationToken);
        }
    }
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.SDK.Common;
using Dolittle.SDK.Events.Processing;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Filters.Builders;

/// <summary>
/// Defines a collection of unregistered event filters.
/// </summary>
public interface IUnregisteredEventFilters : IUniqueBindings<FilterModelId, ICanRegisterEventFilterProcessor>
{
    /// <summary>
    /// Registers all the event filters.
    /// </summary>
    /// <param name="eventProcessors">The <see cref="IEventProcessors" />.</param>
    /// <param name="processingConverter">The <see cref="IEventProcessingConverter"/>.</param>
    /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    void Register(
        IEventProcessors eventProcessors,
        IEventProcessingConverter processingConverter,
        ILoggerFactory loggerFactory,
        CancellationToken cancellationToken);
}

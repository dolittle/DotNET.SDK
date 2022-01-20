// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.SDK.Common;
using Dolittle.SDK.DependencyInversion;
using Dolittle.SDK.Events.Processing;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Handling.Builder;

/// <summary>
/// Defines a collection of unregistered event handlers.
/// </summary>
public interface IUnregisteredEventHandlers : IUniqueBindings<EventHandlerModelId, IEventHandler>
{
    /// <summary>
    /// Gets the callback for configuring the <see cref="ITenantScopedProviders"/>.
    /// </summary>
    ConfigureTenantServices AddTenantScopedServices { get; }
    
    /// <summary>
    /// Registers event handlers.
    /// </summary>
    /// <param name="eventProcessors">The <see cref="IEventProcessors" />.</param>
    /// <param name="processingConverter">The <see cref="IEventProcessingConverter" />.</param>
    /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    void Register(
        IEventProcessors eventProcessors,
        IEventProcessingConverter processingConverter,
        ILoggerFactory loggerFactory,
        CancellationToken cancellationToken);
}

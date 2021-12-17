// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.SDK.DependencyInversion;
using Dolittle.SDK.Events.Processing;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Handling.Builder;

/// <summary>
/// Defines a collection of unregistered event handlers.
/// </summary>
public interface IUnregisteredEventHandlers
{
    /// <summary>
    /// Registers event handlers.
    /// </summary>
    /// <param name="eventProcessors">The <see cref="IEventProcessors" />.</param>
    /// <param name="processingConverter">The <see cref="IEventProcessingConverter" />.</param>
    /// <param name="tenantScopedProvidersBuilder">The <see cref="TenantScopedProvidersBuilder"/>.</param>
    /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
    /// <param name="cancelConnectToken">The <see cref="CancellationToken" />.</param>
    /// <param name="stopProcessingToken">The <see cref="CancellationToken" /> for stopping processing.</param>
    void Register(
        IEventProcessors eventProcessors,
        IEventProcessingConverter processingConverter,
        TenantScopedProvidersBuilder tenantScopedProvidersBuilder,
        ILoggerFactory loggerFactory,
        CancellationToken cancelConnectToken,
        CancellationToken stopProcessingToken);
}

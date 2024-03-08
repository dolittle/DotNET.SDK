// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.SDK.Common;
using Dolittle.SDK.DependencyInversion;
using Dolittle.SDK.Events.Processing;
using Dolittle.SDK.Projections.Store;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Projections.Builder;

/// <summary>
/// Defines a collection of unregistered projections.
/// </summary>
public interface IUnregisteredProjections : IUniqueBindings<ProjectionModelId, IProjection>
{
    
    /// <summary>
    /// Gets the callback for configuring the <see cref="ITenantScopedProviders"/>.
    /// </summary>
    ConfigureTenantServices AddTenantScopedServices { get; }

    /// <summary>
    /// Registers projections.
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
    
    /// <summary>
    /// Gets the <see cref="IProjectionReadModelTypes"/>.
    /// </summary>
    IProjectionReadModelTypes ReadModelTypes { get; }
}

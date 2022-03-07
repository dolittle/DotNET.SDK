// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Aggregates.Builders;
using Dolittle.SDK.Builders;
using Dolittle.SDK.DependencyInversion;
using Dolittle.SDK.Embeddings;
using Dolittle.SDK.EventHorizon;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Store.Builders;
using Dolittle.SDK.Projections.Store.Builders;
using Dolittle.SDK.Resources;
using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK;

/// <summary>
/// Defines the Dolittle Client.
/// </summary>
public interface IDolittleClient
{
    /// <summary>
    /// Gets a value indicating whether the Dolittle Client is connected.
    /// </summary>
    bool IsConnected { get; }
    
    /// <summary>
    /// Gets a <see cref="Task"/> that resolves when the Dolittle Client is connected.
    /// </summary>
    Task Connected { get; }

    /// <summary>
    /// Gets the <see cref="IEventTypes" />.
    /// </summary>
    IEventTypes EventTypes { get; }

    /// <summary>
    /// Gets the <see cref="IEventStoreBuilder" />.
    /// </summary>
    IEventStoreBuilder EventStore { get; }

    /// <summary>
    /// Gets the <see cref="IAggregatesBuilder"/>.
    /// </summary>
    IAggregatesBuilder Aggregates { get; }

    /// <summary>
    /// Gets the <see cref="IProjectionStoreBuilder" />.
    /// </summary>
    IProjectionStoreBuilder Projections { get; }

    /// <summary>
    /// Gets the <see cref="IEmbeddings" />.
    /// </summary>
    IEmbeddings Embeddings { get; }

    /// <summary>
    /// Gets the <see cref="IEventHorizons" />.
    /// </summary>
    IEventHorizons EventHorizons { get; }

    /// <summary>
    /// Gets the <see cref="IEnumerable{T}"/> of <see cref="Tenant"/>.
    /// </summary>
    IEnumerable<Tenant> Tenants { get; }

    /// <summary>
    /// Gets the <see cref="IResourcesBuilder"/>.
    /// </summary>
    IResourcesBuilder Resources { get; }

    /// <summary>
    /// Gets the <see cref="ITenantScopedProviders"/>.
    /// </summary>
    ITenantScopedProviders Services { get; }

    /// <summary>
    /// Connects the <see cref="IDolittleClient"/>.
    /// </summary>
    /// <param name="configuration">The <see cref="DolittleClientConfiguration"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/>, that when resolved, returns the client for continuation.</returns>
    Task<IDolittleClient> Connect(DolittleClientConfiguration configuration, CancellationToken cancellationToken = default);

    /// <summary>
    /// Connects the <see cref="IDolittleClient"/>.
    /// </summary>
    /// <param name="configureClient">The optional <see cref="Action{T}"/> callback for configuring the <see cref="DolittleClientConfiguration"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/>, that when resolved, returns the client for continuation.</returns>
    Task<IDolittleClient> Connect(ConfigureDolittleClient configureClient, CancellationToken cancellationToken = default);

    /// <summary>
    /// Connects the <see cref="IDolittleClient"/>.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/>, that when resolved, returns the client for continuation.</returns>
    Task<IDolittleClient> Connect(CancellationToken cancellationToken = default);

    /// <summary>
    /// Disconnects the <see cref="IDolittleClient"/>.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task Disconnect(CancellationToken cancellationToken = default);
}

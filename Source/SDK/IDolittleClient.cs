// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.DependencyInversion;
using Dolittle.SDK.Embeddings;
using Dolittle.SDK.EventHorizon;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Store;
using Dolittle.SDK.Events.Store.Builders;
using Dolittle.SDK.Projections.Store.Builders;
using Dolittle.SDK.Resources;
using Dolittle.SDK.Tenancy;
using Dolittle.SDK.Tenancy.Client;

namespace Dolittle.SDK
{
    /// <summary>
    /// Defines the Dolittle Client.
    /// </summary>
    public interface IDolittleClient
    {
        /// <summary>
        /// Gets the <see cref="IEventTypes" />.
        /// </summary>
        IEventTypes EventTypes { get; }

        /// <summary>
        /// Gets the <see cref="IEventStoreBuilder" />.
        /// </summary>
        IEventStoreBuilder EventStore { get; }

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
        /// Gets the <see cref="IContainer"/>.
        /// </summary>
        IContainer Services { get; }

        /// <summary>
        /// Start the client.
        /// </summary>
        /// <returns>A <see cref="Task"/> that completes when the client has started. </returns>
        Task Start();

        /// <summary>
        /// Gets the <see cref="IAggregateRootOperations{TAggregate}" /> for a new aggregate of the specified <typeparamref name="TAggregateRoot"/>.
        /// </summary>
        /// <param name="buildEventStore">The <see cref="Func{T, TResult}" /> for creating the <see cref="IEventStore" />.</param>
        /// <typeparam name="TAggregateRoot">The <see cref="Type" /> of the <see cref="AggregateRoot" />.</typeparam>
        /// <returns>The <see cref="IAggregateRootOperations{TAggregate}" />.</returns>
        IAggregateRootOperations<TAggregateRoot> AggregateOf<TAggregateRoot>(Func<IEventStoreBuilder, IEventStore> buildEventStore)
            where TAggregateRoot : AggregateRoot;

        /// <summary>
        /// Gets the <see cref="IAggregateRootOperations{TAggregate}" /> for a new aggregate of the specified <typeparamref name="TAggregateRoot"/>.
        /// </summary>
        /// <param name="eventSource">The <see cref="EventSourceId" />.</param>
        /// <param name="buildEventStore">The <see cref="Func{T, TResult}" /> for creating the <see cref="IEventStore" />.</param>
        /// <typeparam name="TAggregateRoot">The <see cref="Type" /> of the <see cref="AggregateRoot" />.</typeparam>
        /// <returns>The <see cref="IAggregateRootOperations{TAggregate}" />.</returns>
        IAggregateRootOperations<TAggregateRoot> AggregateOf<TAggregateRoot>(EventSourceId eventSource, Func<IEventStoreBuilder, IEventStore> buildEventStore)
            where TAggregateRoot : AggregateRoot;
    }
}
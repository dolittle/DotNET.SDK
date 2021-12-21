// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using BaselineTypeDiscovery;
using Dolittle.SDK.Aggregates.Builders;
using Dolittle.SDK.Embeddings.Builder;
using Dolittle.SDK.Embeddings.Store;
using Dolittle.SDK.EventHorizon;
using Dolittle.SDK.Events.Builders;
using Dolittle.SDK.Events.Filters.Builders;
using Dolittle.SDK.Events.Handling.Builder;
using Dolittle.SDK.Projections.Builder;
using Dolittle.SDK.Projections.Store;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Builders;

/// <summary>
/// Represents an implementation of <see cref="ISetupBuilder"/>.
/// </summary>
public class SetupBuilder : ISetupBuilder
{
    readonly EventTypesBuilder _eventTypesBuilder = new();
    readonly AggregateRootsBuilder _aggregateRootsBuilder = new();
    readonly EventFiltersBuilder _eventFiltersBuilder = new();
    readonly EventHandlersBuilder _eventHandlersBuilder = new();
    readonly SubscriptionsBuilder _eventHorizonsBuilder = new();
    readonly ProjectionReadModelTypeAssociations _projectionAssociations = new();
    readonly EmbeddingReadModelTypeAssociations _embeddingAssociations = new();
    readonly EventSubscriptionRetryPolicy _eventHorizonRetryPolicy = EventHorizonRetryPolicy;
    readonly ProjectionsBuilder _projectionsBuilder;
    readonly EmbeddingsBuilder _embeddingsBuilder;

    bool _withoutDiscovery;

    /// <summary>
    /// Initializes a new instance of the <see cref="SetupBuilder"/> class.
    /// </summary>
    public SetupBuilder()
    {
        _projectionsBuilder = new ProjectionsBuilder(_projectionAssociations);
        _embeddingsBuilder = new EmbeddingsBuilder(_embeddingAssociations);
    }

    /// <inheritdoc />
    public ISetupBuilder WithEventTypes(Action<IEventTypesBuilder> callback)
    {
        callback(_eventTypesBuilder);
        return this;
    }

    /// <inheritdoc />
    public ISetupBuilder WithAggregateRoots(Action<IAggregateRootsBuilder> callback)
    {
        callback(_aggregateRootsBuilder);
        return this;
    }

    /// <inheritdoc />
    public ISetupBuilder WithFilters(Action<IEventFiltersBuilder> callback)
    {
        callback(_eventFiltersBuilder);
        return this;
    }

    /// <inheritdoc />
    public ISetupBuilder WithEventHandlers(Action<IEventHandlersBuilder> callback)
    {
        callback(_eventHandlersBuilder);
        return this;
    }

    /// <inheritdoc />
    public ISetupBuilder WithProjections(Action<ProjectionsBuilder> callback)
    {
        callback(_projectionsBuilder);
        return this;
    }

    /// <inheritdoc />
    public ISetupBuilder WithEmbeddings(Action<EmbeddingsBuilder> callback)
    {
        callback(_embeddingsBuilder);
        return this;
    }

    /// <inheritdoc />
    public ISetupBuilder WithEventHorizons(Action<SubscriptionsBuilder> callback)
    {
        callback(_eventHorizonsBuilder);
        return this;
    }

    /// <inheritdoc />
    public ISetupBuilder WithoutDiscovery()
    {
        _withoutDiscovery = true;
        return this;
    }

    /// <summary>
    /// Builds an unconnected <see cref="DolittleClient"/>.
    /// </summary>
    /// <returns>The <see cref="DolittleClient"/>.</returns>
    public IDolittleClient Build()
    {
        if (!_withoutDiscovery)
        {
            DiscoverAndRegisterAll();
        }

        ApplyAllCallbacks();
        return new DolittleClient(
            _eventTypesBuilder,
            _projectionAssociations,
            _embeddingAssociations,
            _eventHandlersBuilder,
            _aggregateRootsBuilder,
            _projectionsBuilder,
            _embeddingsBuilder,
            _eventFiltersBuilder,
            _eventHorizonsBuilder,
            _eventHorizonRetryPolicy);
    }

    static async Task EventHorizonRetryPolicy(Subscription subscription, ILogger logger, Func<Task<bool>> methodToPerform)
    {
        var retryCount = 0;

        while (!await methodToPerform().ConfigureAwait(false))
        {
            retryCount++;
            var timeout = TimeSpan.FromSeconds(5);
            Log.RetryEventHorizonSubscription(
                logger,
                retryCount,
                timeout,
                subscription.ProducerMicroservice,
                subscription.ProducerTenant,
                subscription.ProducerStream,
                subscription.ProducerPartition,
                subscription.ConsumerTenant,
                subscription.ConsumerScope);
            await Task.Delay(timeout).ConfigureAwait(false);
        }
    }

    static void ForAllAllScannedAssemblies(Action<Assembly> registerAllFromAssembly)
    {
        foreach (var assembly in GetAllAssemblies())
        {
            registerAllFromAssembly(assembly);
        }
    }

    static IEnumerable<Assembly> GetAllAssemblies()
    {
        return AssemblyFinder.FindAssemblies(
            _ => { },
            _ => true,
            false);
    }

    void DiscoverAndRegisterAll()
    {
        RegisterAllEmbeddings();
        RegisterAllProjections();
        RegisterAllAggregateRoots();
        RegisterAllEventHandlers();
        RegisterAllEventTypes();
    }

    void RegisterAllEventTypes() => ForAllAllScannedAssemblies(assembly => _eventTypesBuilder.RegisterAllFrom(assembly));

    void RegisterAllAggregateRoots() => ForAllAllScannedAssemblies(assembly => _aggregateRootsBuilder.RegisterAllFrom(assembly));

    void RegisterAllEventHandlers() => ForAllAllScannedAssemblies(assembly => _eventHandlersBuilder.RegisterAllFrom(assembly));

    void RegisterAllProjections() => ForAllAllScannedAssemblies(assembly => _projectionsBuilder.RegisterAllFrom(assembly));

    void RegisterAllEmbeddings() => ForAllAllScannedAssemblies(assembly => _embeddingsBuilder.RegisterAllFrom(assembly));
}

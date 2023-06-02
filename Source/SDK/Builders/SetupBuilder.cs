// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using BaselineTypeDiscovery;
using Dolittle.SDK.Aggregates.Builders;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Common.Model;
using Dolittle.SDK.EventHorizon;
using Dolittle.SDK.Events.Builders;
using Dolittle.SDK.Events.Filters.Builders;
using Dolittle.SDK.Events.Handling.Builder;
using Dolittle.SDK.Projections.Builder;
using Dolittle.SDK.Projections.Builder.Copies.MongoDB;
using Dolittle.SDK.Projections.Builder.Copies.MongoDB.Internal;
using Dolittle.SDK.Projections.Copies;
using Dolittle.SDK.Projections.Copies.MongoDB;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Builders;

/// <summary>
/// Represents an implementation of <see cref="ISetupBuilder"/>.
/// </summary>
public class SetupBuilder : ISetupBuilder
{
    readonly ModelBuilder _modelBuilder = new();
    readonly ClientBuildResults _buildResults = new();
    readonly EventTypesBuilder _eventTypesBuilder;
    readonly AggregateRootsBuilder _aggregateRootsBuilder;
    readonly EventFiltersBuilder _eventFiltersBuilder;
    readonly EventHandlersBuilder _eventHandlersBuilder;
    readonly ProjectionsBuilder _projectionsBuilder;
    readonly SubscriptionsBuilder _eventHorizonsBuilder = new();
    readonly EventSubscriptionRetryPolicy _eventHorizonRetryPolicy = EventHorizonRetryPolicy;

    bool _withoutDiscovery;

    /// <summary>
    /// Initializes an instance of the <see cref="SetupBuilder"/> class.
    /// </summary>
    public SetupBuilder()
    {
        _eventTypesBuilder = new EventTypesBuilder(_modelBuilder, _buildResults);
        _aggregateRootsBuilder = new AggregateRootsBuilder(_modelBuilder, _buildResults);
        _eventFiltersBuilder = new EventFiltersBuilder(_modelBuilder);
        _eventHandlersBuilder = new EventHandlersBuilder(_modelBuilder, _buildResults);

        
        _projectionsBuilder = new ProjectionsBuilder(
            _modelBuilder,
            _buildResults,
            new ProjectionCopyToMongoDbBuilderFactory(
                new MongoDbCollectionNameValidator(),
                new ConversionsFromBsonClassMapBuilder(new ConversionsFromBsonClassMapAdder()),
                new ConversionsFromConvertToMongoDBAttributesBuilder(),
                new PropertyPathResolver()));
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
    public ISetupBuilder WithProjections(Action<IProjectionsBuilder> callback)
    {
        callback(_projectionsBuilder);
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
        
        var model = _modelBuilder.Build(_buildResults);
        var unregisteredEventTypes = EventTypesBuilder.Build(model, _buildResults);
        return new DolittleClient(
            _buildResults,
            unregisteredEventTypes,
            AggregateRootsBuilder.Build(model, _buildResults),
            EventFiltersBuilder.Build(model, _buildResults),
            EventHandlersBuilder.Build(model, unregisteredEventTypes, _buildResults),
            ProjectionsBuilder.Build(model, unregisteredEventTypes, _buildResults),
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
            logger.RetryEventHorizonSubscription(
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
        RegisterAllEventTypes();
        RegisterAllAggregateRoots();
        RegisterAllEventHandlers();
        RegisterAllProjections();
    }

    void RegisterAllEventTypes() => ForAllAllScannedAssemblies(assembly => _eventTypesBuilder.RegisterAllFrom(assembly));

    void RegisterAllAggregateRoots() => ForAllAllScannedAssemblies(assembly => _aggregateRootsBuilder.RegisterAllFrom(assembly));

    void RegisterAllEventHandlers() => ForAllAllScannedAssemblies(assembly => _eventHandlersBuilder.RegisterAllFrom(assembly));

    void RegisterAllProjections() => ForAllAllScannedAssemblies(assembly => _projectionsBuilder.RegisterAllFrom(assembly));
}

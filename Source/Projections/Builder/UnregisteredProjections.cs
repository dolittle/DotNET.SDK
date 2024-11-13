// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using System.Threading;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.SDK.Common;
using Dolittle.SDK.DependencyInversion;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Handling.Internal;
using Dolittle.SDK.Events.Processing;
using Dolittle.SDK.Events.Processing.Internal;
using Dolittle.SDK.Projections.Actors;
using Dolittle.SDK.Projections.Copies.MongoDB;
using Dolittle.SDK.Projections.Internal;
using Dolittle.SDK.Projections.Store;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Proto;

namespace Dolittle.SDK.Projections.Builder;

/// <summary>
/// Represents an implementation of <see cref="IUnregisteredProjections"/>.
/// </summary>
public class UnregisteredProjections : UniqueBindings<ProjectionModelId, IProjection>, IUnregisteredProjections
{
    /// <summary>
    /// Initializes an instance of the <see cref="UnregisteredProjections"/> class.
    /// </summary>
    /// <param name="projections">The unique <see cref="IProjection"/> projections.</param>
    /// <param name="readModelTypes">The <see cref="IProjectionReadModelTypes"/>.</param>
    public UnregisteredProjections(IUniqueBindings<ProjectionModelId, IProjection> projections,
        IProjectionReadModelTypes readModelTypes)
        : base(projections)
    {
        ReadModelTypes = readModelTypes;
        AddTenantScopedServices = AddToContainer;
    }

    /// <inheritdoc />
    public ConfigureTenantServices AddTenantScopedServices { get; }

    /// <inheritdoc />
    public void Register(
        IEventProcessors eventProcessors,
        IEventProcessingConverter processingConverter,
        ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        foreach (var projection in Values)
        {
            eventProcessors.Register(
                CreateProjectionsProcessor(
                    projection,
                    processingConverter,
                    loggerFactory),
                EventHandlerProtocol.Instance,
                cancellationToken);
        }
    }

    /// <inheritdoc />
    public IProjectionReadModelTypes ReadModelTypes { get; }

    static EventProcessor<ProjectionId, EventHandlerRegistrationRequest, HandleEventRequest, EventHandlerResponse>
        CreateProjectionsProcessor(
            IProjection projection,
            IEventProcessingConverter processingConverter,
            ILoggerFactory loggerFactory)
    {
        var processorType = typeof(ProjectionsProcessor<>).MakeGenericType(projection.ProjectionType);
        // var clientType = typeof(IProjectionClient<>).MakeGenericType(projection.ProjectionType);
        var instance = Activator.CreateInstance(
                           processorType,
                           projection,
                           processingConverter,
                           loggerFactory.CreateLogger(processorType)) ??
                       throw new InvalidOperationException($"Could not create an instance of {processorType}");
        return (EventProcessor<ProjectionId, EventHandlerRegistrationRequest, HandleEventRequest, EventHandlerResponse>)
            instance;
    }

    void AddToContainer(TenantId tenantId, IServiceCollection serviceCollection)
    {
        foreach (var projection in Values)
        {
            var readModelType = projection.ProjectionType;

            var collectionName = MongoDBProjectionCollectionName.From(projection.Identifier, projection.Alias?.Value)
                .Value;
            serviceCollection.AddScoped(
                typeof(IMongoCollection<>).MakeGenericType(readModelType),
                serviceProvider => GetCollectionMethodForReadModel(readModelType).Invoke(
                    serviceProvider.GetRequiredService<IMongoDatabase>(), [collectionName, null]));

            var projectionClientInterface = typeof(IProjectionClient<>).MakeGenericType(readModelType);
            var projectionClientType = typeof(ProjectionClient<>).MakeGenericType(readModelType);
            serviceCollection.AddSingleton(
                projectionClientInterface,
                serviceProvider =>
                    Activator.CreateInstance(projectionClientType, [
                        projection,
                        serviceProvider.GetRequiredService<IRootContext>(),
                        tenantId
                    ])!);

            serviceCollection.AddScoped(
                typeof(IProjectionOf<>).MakeGenericType(readModelType),
                serviceProvider => GetOfMethodForReadModel(readModelType).Invoke(
                    serviceProvider.GetRequiredService<IProjectionStore>(),
                    [
                        projection.Identifier,
                        projection.ScopeId
                    ])!);
        }
    }

    static MethodInfo GetOfMethodForReadModel(Type readModelType)
        => typeof(IProjectionStore).GetMethod(
                nameof(IProjectionStore.Of),
                [
                    typeof(ProjectionId),
                    typeof(ScopeId)
                ])
            ?.MakeGenericMethod(readModelType);

    static MethodInfo GetCollectionMethodForReadModel(Type readModelType)
        => typeof(IMongoDatabase).GetMethod(
                nameof(IMongoDatabase.GetCollection),
                [
                    typeof(string),
                    typeof(MongoCollectionSettings)
                ])
            ?.MakeGenericMethod(readModelType);
}

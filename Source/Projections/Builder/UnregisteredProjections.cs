// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.SDK.Common;
using Dolittle.SDK.DependencyInversion;
using Dolittle.SDK.Events.Processing;
using Dolittle.SDK.Events.Processing.Internal;
using Dolittle.SDK.Projections.Internal;
using Dolittle.SDK.Projections.Store;
using Dolittle.SDK.Projections.Store.Converters;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
    public UnregisteredProjections(IUniqueBindings<ProjectionModelId, IProjection> projections, IProjectionReadModelTypes readModelTypes)
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
        IConvertProjectionsToSDK projectionsConverter,
        ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        foreach (var projection in Values)
        {
            eventProcessors.Register(
                CreateProjectionsProcessor(
                    projection,
                    processingConverter,
                    projectionsConverter,
                    loggerFactory),
                new ProjectionsProtocol(),
                cancellationToken);
        }
    }

    /// <inheritdoc />
    public IProjectionReadModelTypes ReadModelTypes { get; }

    static EventProcessor<ProjectionId, ProjectionRegistrationRequest, ProjectionRequest, ProjectionResponse> CreateProjectionsProcessor(
        IProjection projection,
        IEventProcessingConverter processingConverter,
        IConvertProjectionsToSDK projectionConverter,
        ILoggerFactory loggerFactory)
    {
        var processorType = typeof(ProjectionsProcessor<>).MakeGenericType(projection.ProjectionType); 
        return Activator.CreateInstance(
            processorType,
            projection,
            processingConverter,
            projectionConverter,
            loggerFactory.CreateLogger(processorType)) as EventProcessor<ProjectionId, ProjectionRegistrationRequest, ProjectionRequest, ProjectionResponse>;
    }
    void AddToContainer(TenantId tenantId, IServiceCollection serviceCollection)
    {
        foreach (var projection in Values)
        {
            var readModelType = projection.ProjectionType;
            serviceCollection.AddSingleton(
                typeof(IProjectionOf<>).MakeGenericType(readModelType),
                serviceProvider => Activator.CreateInstance(
                    typeof(ProjectionOf<>).MakeGenericType(readModelType),
                    serviceProvider.GetService<IProjectionStore>(),
                    new ScopedProjectionId(projection.Identifier, projection.ScopeId)) ?? throw new CouldNotCreateProjectionOf(projection.ProjectionType, tenantId));
        }
    }
}

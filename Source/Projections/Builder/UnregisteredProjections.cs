// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.SDK.Common;
using Dolittle.SDK.Events.Processing;
using Dolittle.SDK.Events.Processing.Internal;
using Dolittle.SDK.Projections.Internal;
using Dolittle.SDK.Projections.Store;
using Dolittle.SDK.Projections.Store.Converters;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Projections.Builder;

/// <summary>
/// Represents an implementation of <see cref="IUnregisteredProjections"/>.
/// </summary>
public class UnregisteredProjections : UniqueBindings<ProjectionId, IProjection>, IUnregisteredProjections
{
    /// <summary>
    /// Initializes an instance of the <see cref="UnregisteredProjections"/> class.
    /// </summary>
    /// <param name="eventHandlers">The unique <see cref="IProjection"/> projections.</param>
    /// <param name="readModelTypes">The <see cref="IProjectionReadModelTypes"/>.</param>
    public UnregisteredProjections(IUniqueBindings<ProjectionId, IProjection> eventHandlers, IProjectionReadModelTypes readModelTypes)
        : base(eventHandlers)
    {
        ReadModelTypes = readModelTypes;
    }

    /// <inheritdoc />
    public void Register(
        IEventProcessors eventProcessors,
        IEventProcessingConverter processingConverter,
        IConvertProjectionsToSDK projectionsConverter,
        ILoggerFactory loggerFactory,
        CancellationToken cancelConnectToken,
        CancellationToken stopProcessingToken)
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
                cancelConnectToken,
                stopProcessingToken);
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
}

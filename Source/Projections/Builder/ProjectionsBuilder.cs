// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reflection;
using Dolittle.SDK.Common;
using Dolittle.SDK.ApplicationModel.ClientSetup;
using Dolittle.SDK.ApplicationModel;
using Dolittle.SDK.Events;
using Dolittle.SDK.Projections.Builder.Copies.MongoDB.Internal;
using Dolittle.SDK.Projections.Store;

namespace Dolittle.SDK.Projections.Builder;

/// <summary>
/// Represents the builder for configuring event handlers.
/// </summary>
public class ProjectionsBuilder : IProjectionsBuilder
{
    readonly IModelBuilder _modelBuilder;
    readonly IProjectionCopyToMongoDBBuilderFactory _copyToMongoDbBuilderFactory;
    readonly DecoratedTypeBindingsToModelAdder<ProjectionAttribute, ProjectionModelId, ProjectionId> _decoratedTypeBindings;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionsBuilder"/> class.
    /// </summary>
    /// <param name="modelBuilder">The <see cref="IModelBuilder"/>.</param>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    /// <param name="copyToMongoDbBuilderFactory">The <see cref="IProjectionCopyToMongoDBBuilderFactory"/>.</param>
    public ProjectionsBuilder(
        IModelBuilder modelBuilder,
        IClientBuildResults buildResults,
        IProjectionCopyToMongoDBBuilderFactory copyToMongoDbBuilderFactory)
    {
        _modelBuilder = modelBuilder;
        _copyToMongoDbBuilderFactory = copyToMongoDbBuilderFactory;
        _decoratedTypeBindings = new DecoratedTypeBindingsToModelAdder<ProjectionAttribute, ProjectionModelId, ProjectionId>("projection", modelBuilder, buildResults);
    }


    /// <inheritdoc />
    public IProjectionBuilder Create(ProjectionId projectionId)
    {
        var builder = new ProjectionBuilder(projectionId, _modelBuilder, _copyToMongoDbBuilderFactory);
        return builder;
    }

    /// <inheritdoc />
    public IProjectionsBuilder Register<TProjection>()
        where TProjection : class, new()
        => Register(typeof(TProjection));


    /// <inheritdoc />
    public IProjectionsBuilder Register(Type type)
    {
        if (!_decoratedTypeBindings.TryAdd(type, out var identifier))
        {
            return this;
        }
        BindBuilder(type, identifier.Identifier);
        return this;
    }

    /// <inheritdoc />
    public IProjectionsBuilder RegisterAllFrom(Assembly assembly)
    {
        var addedEventHandlerBindings = _decoratedTypeBindings.AddFromAssembly(assembly);
        foreach (var (identifier, type) in addedEventHandlerBindings)
        {
            BindBuilder(type, identifier);
        }

        return this;
    }

    void BindBuilder(Type type, ProjectionModelId identifier)
        =>  _modelBuilder.BindIdentifierToProcessorBuilder<ICanTryBuildProjection, ProjectionModelId, ProjectionId>(
            identifier,
            CreateConventionProjectionBuilderFor(type));

    ICanTryBuildProjection CreateConventionProjectionBuilderFor(Type readModelType)
        => Activator.CreateInstance(
                typeof(ConventionProjectionBuilder<>).MakeGenericType(readModelType),
                CreateForMethodForReadModel(readModelType).Invoke(_copyToMongoDbBuilderFactory, Array.Empty<object>()))
            as ICanTryBuildProjection;
    
    
    static MethodInfo CreateForMethodForReadModel(Type readModelType)
        => typeof(IProjectionCopyToMongoDBBuilderFactory).GetMethod(
                nameof(IProjectionCopyToMongoDBBuilderFactory.CreateFor),
                Array.Empty<Type>())
            ?.MakeGenericMethod(readModelType);
    

    /// <summary>
    /// Build projections.
    /// </summary>
    /// <param name="applicationModel">The <see cref="IApplicationModel"/>.</param>
    /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
    /// <param name="buildResults">The <see cref="IClientBuildResults" />.</param>
    public static IUnregisteredProjections Build(IApplicationModel applicationModel, IEventTypes eventTypes, IClientBuildResults buildResults)
    {
        var projections = new UniqueBindings<ProjectionModelId, IProjection>();
        foreach (var (id, builder) in applicationModel.GetProcessorBuilderBindings<ICanTryBuildProjection, ProjectionModelId, ProjectionId>())
        {
            if (builder.TryBuild(id, eventTypes, buildResults, out var projection))
            {
                projections.Add(id, projection);
            }
        }
        var identifiers = applicationModel.GetTypeBindings<ProjectionModelId, ProjectionId>();
        var readModelTypes = new ProjectionReadModelTypes();
        foreach (var (identifier, type) in identifiers)
        {
            readModelTypes.Add(new ScopedProjectionId(identifier.Id, identifier.Scope), type);
        }
        return new UnregisteredProjections(projections, readModelTypes);
    }
}

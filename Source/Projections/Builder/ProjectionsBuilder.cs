// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using Dolittle.SDK.Common;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Common.Model;
using Dolittle.SDK.Events;
using Dolittle.SDK.Projections.Store;

namespace Dolittle.SDK.Projections.Builder;

/// <summary>
/// Represents the builder for configuring event handlers.
/// </summary>
public class ProjectionsBuilder : IProjectionsBuilder
{
    readonly IModelBuilder _modelBuilder;
    readonly DecoratedTypeBindingsToModelAdder<ProjectionAttribute, ProjectionModelId, ProjectionId> _decoratedTypeBindings;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionsBuilder"/> class.
    /// </summary>
    /// <param name="modelBuilder">The <see cref="IModelBuilder"/>.</param>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    /// <param name="copyToMongoDbBuilderFactory">The <see cref="IProjectionCopyToMongoDBBuilderFactory"/>.</param>
    public ProjectionsBuilder(
        IModelBuilder modelBuilder,
        IClientBuildResults buildResults)
    {
        _modelBuilder = modelBuilder;
        _decoratedTypeBindings = new DecoratedTypeBindingsToModelAdder<ProjectionAttribute, ProjectionModelId, ProjectionId>("projection", modelBuilder, buildResults);
    }


    /// <inheritdoc />
    public IProjectionBuilder Create(ProjectionId projectionId)
    {
        var builder = new ProjectionBuilder(projectionId, _modelBuilder);
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
        BindConventionBuilder(type, identifier);
        return this;
    }

    /// <inheritdoc />
    public IProjectionsBuilder RegisterAllFrom(Assembly assembly)
    {
        var addedEventHandlerBindings = _decoratedTypeBindings.AddFromAssembly(assembly);
        foreach (var (type, decorator) in addedEventHandlerBindings)
        {
            BindConventionBuilder(type, decorator);
        }

        return this;
    }

    void BindConventionBuilder(Type type, ProjectionModelId identifier)
        =>  _modelBuilder.BindIdentifierToProcessorBuilder(
            identifier,
            CreateConventionProjectionBuilderFor(type, identifier));

    ICanTryBuildProjection CreateConventionProjectionBuilderFor(Type readModelType, ProjectionModelId identifier)
        => Activator.CreateInstance(
                typeof(ConventionProjectionBuilder<>).MakeGenericType(readModelType),
                identifier)
            as ICanTryBuildProjection;
    
    
    // static MethodInfo CreateForMethodForReadModel(Type readModelType)
    //     => typeof(IProjectionCopyToMongoDBBuilderFactory).GetMethod(nameof(IProjectionCopyToMongoDBBuilderFactory.CreateFor), Array.Empty<Type>())?.MakeGenericMethod(readModelType);
    

    /// <summary>
    /// Build projections.
    /// </summary>
    /// <param name="model">The <see cref="IModel"/>.</param>
    /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
    /// <param name="buildResults">The <see cref="IClientBuildResults" />.</param>
    public static IUnregisteredProjections Build(IModel model, IEventTypes eventTypes, IClientBuildResults buildResults)
    {
        var projections = new UniqueBindings<ProjectionModelId, IProjection>();
        foreach (var (identifier, builder) in model.GetProcessorBuilderBindings<ICanTryBuildProjection>())
        {
            
            if (builder.TryBuild((ProjectionModelId)identifier, eventTypes, buildResults, out var projection))
            {
                projections.Add((ProjectionModelId)identifier, projection);
            }
        }
        var identifiers = model.GetTypeBindings<ProjectionModelId, ProjectionId>();
        var readModelTypes = new ProjectionReadModelTypes();
        foreach (var identifier in identifiers)
        {
            readModelTypes.Add(identifier.Identifier, identifier.Type);
        }
        return new UnregisteredProjections(projections, readModelTypes);
    }
}

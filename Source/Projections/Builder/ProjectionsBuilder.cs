// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dolittle.SDK.Common;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Common.Model;
using Dolittle.SDK.Events;



namespace Dolittle.SDK.Projections.Builder;

/// <summary>
/// Represents the builder for configuring event handlers.
/// </summary>
public class ProjectionsBuilder : IProjectionsBuilder
{
    readonly IModelBuilder _modelBuilder;
    readonly IClientBuildResults _buildResults;
    readonly DecoratedTypeBindingsToModelAdder<ProjectionAttribute, ProjectionModelId, ProjectionId> _decoratedTypeBindings;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionsBuilder"/> class.
    /// </summary>
    /// <param name="modelBuilder">The <see cref="IModelBuilder"/>.</param>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    public ProjectionsBuilder(IModelBuilder modelBuilder, IClientBuildResults buildResults)
    {
        _modelBuilder = modelBuilder;
        _buildResults = buildResults;
        _decoratedTypeBindings = new DecoratedTypeBindingsToModelAdder<ProjectionAttribute, ProjectionModelId, ProjectionId>("projection", modelBuilder, buildResults);
    }


    /// <inheritdoc />
    public IProjectionBuilder CreateProjection(ProjectionId projectionId)
    {
        var builder = new ProjectionBuilder(projectionId, _modelBuilder);
        return builder;
    }

    /// <inheritdoc />
    public IProjectionsBuilder RegisterProjection<TProjection>()
        where TProjection : class, new()
        => RegisterProjection(typeof(TProjection));


    /// <inheritdoc />
    public IProjectionsBuilder RegisterProjection(Type type)
    {
        if (!_decoratedTypeBindings.TryAdd(type, out var decorator))
        {
            return this;
        }
        BindBuilder(type, decorator);
        return this;
    }

    /// <inheritdoc />
    public IProjectionsBuilder RegisterAllFrom(Assembly assembly)
    {
        var addedEventHandlerBindings = _decoratedTypeBindings.AddFromAssembly(assembly);
        foreach (var (type, decorator) in addedEventHandlerBindings)
        {
            BindBuilder(type, decorator);
        }

        return this;
    }

    void BindBuilder(Type type, ProjectionAttribute decorator)
        =>  _modelBuilder.BindIdentifierToProcessorBuilder(
            decorator.GetIdentifier(),
            CreateConventionProjectionBuilderFor(type, decorator));
    
    static ICanTryBuildProjection CreateConventionProjectionBuilderFor(Type type, ProjectionAttribute decorator)
        => Activator.CreateInstance(
                typeof(ConventionProjectionBuilder<>).MakeGenericType(type),
                decorator)
            as ICanTryBuildProjection;

    /// <summary>
    /// Build projections.
    /// </summary>
    /// <param name="model">The <see cref="IModel"/>.</param>
    /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
    /// <param name="buildResults">The <see cref="IClientBuildResults" />.</param>
    public IUnregisteredProjections Build(
        IModel model,
        IEventTypes eventTypes,
        IClientBuildResults buildResults)
    {
        var builders = model.GetProcessorBuilderBindings<ICanTryBuildProjection>();
        var projections = new List<IProjection>();
        foreach (var builder in builders.Select(_ => _.ProcessorBuilder))
        {
            if (builder.TryBuild(eventTypes, buildResults, out var projection))
            {
                projections.Add(projection);
            }
        }
        return new UnregisteredProjections(new UniqueBindings<ProjectionId, IProjection>(projections.ToDictionary(_ => _.Identifier, _ => _)));
    }
}

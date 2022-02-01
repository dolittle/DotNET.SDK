// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reflection;
using Dolittle.SDK.Common;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Common.Model;
using Dolittle.SDK.Events;
using Dolittle.SDK.Projections.Copies;
using Dolittle.SDK.Projections.Store;

namespace Dolittle.SDK.Projections.Builder;

/// <summary>
/// Represents the builder for configuring event handlers.
/// </summary>
public class ProjectionsBuilder : IProjectionsBuilder
{
    readonly IModelBuilder _modelBuilder;
    readonly IProjectionCopiesFromReadModelBuilders _projectionCopiesFromReadModelBuilder;
    readonly DecoratedTypeBindingsToModelAdder<ProjectionAttribute, ProjectionModelId, ProjectionId> _decoratedTypeBindings;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionsBuilder"/> class.
    /// </summary>
    /// <param name="modelBuilder">The <see cref="IModelBuilder"/>.</param>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    public ProjectionsBuilder(IModelBuilder modelBuilder, IClientBuildResults buildResults, IProjectionCopiesFromReadModelBuilders projectionCopiesFromReadModelBuilder)
    {
        _modelBuilder = modelBuilder;
        _projectionCopiesFromReadModelBuilder = projectionCopiesFromReadModelBuilder;
        _decoratedTypeBindings = new DecoratedTypeBindingsToModelAdder<ProjectionAttribute, ProjectionModelId, ProjectionId>("projection", modelBuilder, buildResults);
    }


    /// <inheritdoc />
    public IProjectionBuilder Create(ProjectionId projectionId)
    {
        var builder = new ProjectionBuilder(projectionId, _modelBuilder, _projectionCopiesFromReadModelBuilder);
        return builder;
    }

    /// <inheritdoc />
    public IProjectionsBuilder Register<TProjection>()
        where TProjection : class, new()
        => Register(typeof(TProjection));


    /// <inheritdoc />
    public IProjectionsBuilder Register(Type type)
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

    ICanTryBuildProjection CreateConventionProjectionBuilderFor(Type type, ProjectionAttribute decorator)
        => Activator.CreateInstance(
                typeof(ConventionProjectionBuilder<>).MakeGenericType(type),
                decorator,
                _projectionCopiesFromReadModelBuilder)
            as ICanTryBuildProjection;

    /// <summary>
    /// Build projections.
    /// </summary>
    /// <param name="model">The <see cref="IModel"/>.</param>
    /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
    /// <param name="buildResults">The <see cref="IClientBuildResults" />.</param>
    public static IUnregisteredProjections Build(IModel model, IEventTypes eventTypes, IClientBuildResults buildResults)
    {
        var projections = new UniqueBindings<ProjectionModelId, IProjection>();
        foreach (var builder in model.GetProcessorBuilderBindings<ICanTryBuildProjection>().Select(_ => _.ProcessorBuilder))
        {
            if (builder.TryBuild(eventTypes, buildResults, out var projection))
            {
                projections.Add(new ProjectionModelId(projection.Identifier, projection.ScopeId), projection);
            }
        }
        var identifiers = model.GetTypeBindings<ProjectionModelId, ProjectionId>();
        var readModelTypes = new ProjectionReadModelTypes();
        foreach (var (identifier, type) in identifiers)
        {
            readModelTypes.Add(new ScopedProjectionId(identifier.Id, identifier.Scope), type);
        }
        return new UnregisteredProjections(projections, readModelTypes);
    }
}

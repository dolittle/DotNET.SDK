// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Common.Model;
using Dolittle.SDK.Events;


namespace Dolittle.SDK.Projections.Builder;

/// <summary>
/// Represents a builder for building projections.
/// </summary>
public class ProjectionBuilder : IProjectionBuilder, ICanTryBuildProjection
{
    readonly ProjectionId _projectionId;
    readonly IModelBuilder _modelBuilder;
    readonly ICreateProjection _projectionCreator;
    ICanTryBuildProjection _methodsBuilder;

    ScopeId _scopeId = ScopeId.Default;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionBuilder"/> class.
    /// </summary>
    /// <param name="projectionId">The <see cref="ProjectionId" />.</param>
    /// <param name="modelBuilder">The <see cref="IModelBuilder" />.</param>
    /// <param name="projectionCreator">The <see cref="ICreateProjection"/>.</param>
    public ProjectionBuilder(ProjectionId projectionId, IModelBuilder modelBuilder, ICreateProjection projectionCreator)
    {
        _projectionId = projectionId;
        _modelBuilder = modelBuilder;
        _projectionCreator = projectionCreator;
    }

    /// <inheritdoc />
    public bool Equals(ICanTryBuildProjection other) => ReferenceEquals(this, other);

    /// <inheritdoc />
    public IProjectionBuilder InScope(ScopeId scopeId)
    {
        _scopeId = scopeId;
        return this;
    }

    /// <inheritdoc />
    public IProjectionBuilderForReadModel<TReadModel> ForReadModel<TReadModel>()
        where TReadModel : class, new()
    {
        if (_methodsBuilder != default)
        {
            throw new ReadModelAlreadyDefinedForProjection(_projectionId, _scopeId, typeof(TReadModel));
        }
        
        var builder = new ProjectionBuilderForReadModel<TReadModel>(_projectionId, _scopeId, _modelBuilder, _projectionCreator, this);
        _methodsBuilder = builder;
        return builder;
    }

    /// <inheritdoc/>
    public bool TryBuild(IEventTypes eventTypes, IClientBuildResults buildResults, out IProjection projection)
    {
        projection = default;
        if (_methodsBuilder != null)
        {
            return _methodsBuilder.TryBuild(eventTypes, buildResults, out projection);
        }
        buildResults.AddFailure($"Failed to build projection {_projectionId}. No read model defined for projection.");
        return false;

    }
}

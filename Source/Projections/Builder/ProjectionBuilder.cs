// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
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
    ICanTryBuildProjection? _methodsBuilder;

    ScopeId _scopeId = ScopeId.Default;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionBuilder"/> class.
    /// </summary>
    /// <param name="projectionId">The <see cref="ProjectionId" />.</param>
    /// <param name="modelBuilder">The <see cref="IModelBuilder" />.</param>
    /// <param name="copyToMongoDBBuilderFactory">The <see cref="IProjectionCopyToMongoDBBuilderFactory"/>.</param>
    public ProjectionBuilder(ProjectionId projectionId, IModelBuilder modelBuilder)
    {
        _projectionId = projectionId;
        _modelBuilder = modelBuilder;
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
        where TReadModel : ReadModel, new()
    {
        if (_methodsBuilder != default)
        {
            throw new ReadModelAlreadyDefinedForProjection(_projectionId, _scopeId, typeof(TReadModel));
        }
        
        var builder = new ProjectionBuilderForReadModel<TReadModel>(
            _projectionId,
            _scopeId,
            _modelBuilder,
            this);
        _methodsBuilder = builder;
        return builder;
    }

    /// <inheritdoc/>
    public bool TryBuild(ProjectionModelId identifier, IEventTypes eventTypes, IClientBuildResults buildResults, [NotNullWhen(true)] out IProjection? projection)
    {
        projection = default;
        if (_methodsBuilder != null)
        {
            return _methodsBuilder.TryBuild(identifier, eventTypes, buildResults, out projection);
        }
        buildResults.AddFailure(identifier, $"No read model defined for projection.");
        return false;

    }
}

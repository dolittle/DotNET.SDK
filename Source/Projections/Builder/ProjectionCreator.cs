// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Events;
using Dolittle.SDK.Projections.Copies;

namespace Dolittle.SDK.Projections.Builder;

/// <summary>
/// Represents an implementation of <see cref="ICreateProjection"/>.
/// </summary>
public class ProjectionCreator : ICreateProjection
{
    readonly IResolveProjectionCopiesDefinition _projectionCopiesResolver;

    /// <summary>
    /// Initializes a new instance of thee <see cref="ProjectionCreator"/> class.
    /// </summary>
    /// <param name="projectionCopiesResolver">The <see cref="IResolveProjectionCopiesDefinition" />.</param>
    public ProjectionCreator(IResolveProjectionCopiesDefinition projectionCopiesResolver)
    {
        _projectionCopiesResolver = projectionCopiesResolver;
    }

    /// <inheritdoc />
    public bool TryCreate<TReadModel>(
        ProjectionId identifier,
        ScopeId scopeId,
        IDictionary<EventType, IProjectionMethod<TReadModel>> onMethods,
        IClientBuildResults buildResults,
        out IProjection projection)
        where TReadModel : class, new()
    {
        projection = default;
        if (!_projectionCopiesResolver.TryResolveFor<TReadModel>(buildResults, out var copies))
        {
            return false;
        }
        projection = new Projection<TReadModel>(identifier, scopeId, onMethods, copies);
        return true;
    }
}

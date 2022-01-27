// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections.Builder;

/// <summary>
/// Defines a factory for <see cref="IProjection{TReadModel}"/>.
/// </summary>
public interface ICreateProjection
{
    /// <summary>
    /// Try to create an <see cref="IProjection{TReadModel}"/> of <typeparamref name="TReadModel"/>.
    /// </summary>
    /// <param name="identifier">The <see cref="ProjectionId" />.</param>
    /// <param name="scopeId">The <see cref="ScopeId" />.</param>
    /// <param name="onMethods">The on methods by <see cref="EventType" />.</param>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    /// <param name="projection">The created <see cref="IProjection"/>.</param>
    /// <typeparam name="TReadModel">The type of the projection read model.</typeparam>
    /// <returns>True if <see cref="IProjection{TReadModel}"/> was successfully created, false if not.</returns>
    bool TryCreate<TReadModel>(
        ProjectionId identifier,
        ScopeId scopeId,
        IDictionary<EventType, IProjectionMethod<TReadModel>> onMethods,
        IClientBuildResults buildResults,
        out IProjection projection)
        where TReadModel : class, new();
}

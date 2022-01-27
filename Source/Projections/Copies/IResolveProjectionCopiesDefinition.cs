// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.SDK.Common.ClientSetup;

namespace Dolittle.SDK.Projections.Copies;

/// <summary>
/// Defines a system that can resolve <see cref="ProjectionCopies"/> to be used in a <see cref="ProjectionRegistrationRequest"/>.
/// </summary>
public interface IResolveProjectionCopiesDefinition
{
    /// <summary>
    /// Try resolve the <see cref="ProjectionCopies"/> argument for the <see cref="ProjectionRegistrationRequest"/> from the <see cref="IProjection{TReadModel}"/> of <typeparamref name="TProjection"/>.
    /// </summary>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    /// <param name="copies">The resolved <see cref="ProjectionCopies"/>.</param>
    /// <typeparam name="TProjection">The <see cref="Type"/> of the projection read model.</typeparam>
    /// <returns>True if resolved, false if not..</returns>
    bool TryResolveFor<TProjection>(IClientBuildResults buildResults, out ProjectionCopies copies)
        where TProjection : class, new();
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Common.ClientSetup;

namespace Dolittle.SDK.Projections.Copies;

/// <summary>
/// Defines a system that can augment an instance 
/// </summary>
public interface ICanAugmentProjectionCopy
{
    /// <summary>
    /// Adds to the given <see cref="ProjectionCopies"/>.
    /// </summary>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    /// <param name="projectionCopies">THe <see cref="ProjectionCopies"/> to use as a base for the returned <see cref="ProjectionCopies"/>.</param>
    /// <param name="augmentedProjectionCopies">The augmented <see cref="ProjectionCopies"/>.</param>
    /// <typeparam name="TProjection">The <see cref="Type"/> of the projection read model.</typeparam>
    /// <returns>The augmented <see cref="ProjectionCopies"/>.</returns>
    bool TryAugment<TProjection>(IClientBuildResults buildResults, ProjectionCopies projectionCopies, out ProjectionCopies augmentedProjectionCopies)
        where TProjection : class, new();
}

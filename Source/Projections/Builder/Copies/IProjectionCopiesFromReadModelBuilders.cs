// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Projections.Copies;

namespace Dolittle.SDK.Projections.Builder.Copies;

/// <summary>
/// Defines a system that can build <see cref="ProjectionCopies"/> from the given projection read model <see cref="Type"/>.
/// </summary>
public interface IProjectionCopiesFromReadModelBuilders
{
    /// <summary>
    /// Try build the <see cref="ProjectionCopies"/> from the <typeparamref name="TReadModel"/>.
    /// </summary>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    /// <param name="projectionCopies">The resolved <see cref="ProjectionCopies"/>.</param>
    /// <typeparam name="TReadModel">The <see cref="Type"/> of the projection read model.</typeparam>
    /// <returns>True if successfully resolved, false if not.</returns>
    bool TryBuildFrom<TReadModel>(IClientBuildResults buildResults, out ProjectionCopies projectionCopies)
        where TReadModel : class, new();
}

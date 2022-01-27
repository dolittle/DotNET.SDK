// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Dolittle.SDK.Projections.Copies.MongoDB;

/// <summary>
/// Defines a system that can resolve the BSON conversions for an <see cref="IProjection{TReadModel}"/> for a projection read model <see cref="Type"/>.
/// </summary>
public interface IResolveConversions
{
    /// <summary>
    /// Resolves the BSON conversions for fields of the <typeparamref name="TProjection"/> projection read model <see cref="Type"/>.
    /// </summary>
    /// <param name="projection">The <see cref="IProjection{TReadModel}"/>.</param>
    /// <typeparam name="TProjection">The <see cref="Type"/> of the projection read model.</typeparam>
    /// <returns>The conversions per field.</returns>
    IDictionary<string, Runtime.Events.Processing.Contracts.ProjectionCopyToMongoDB.Types.BSONType> Resolve<TProjection>(IProjection<TProjection> projection)
        where TProjection : class, new();
}

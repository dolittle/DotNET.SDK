// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.SDK.Common.ClientSetup;
using MongoDB.Bson;

namespace Dolittle.SDK.Projections.Copies.MongoDB;

/// <summary>
/// Defines a system that can resolve the BSON conversions for an <see cref="IProjection{TReadModel}"/> for a projection read model <see cref="Type"/>.
/// </summary>
public interface IResolveConversions
{
    /// <summary>
    /// Resolves the BSON conversions for fields of the <typeparamref name="TProjection"/> projection read model <see cref="Type"/>.
    /// </summary>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    /// <param name="conversions">The resolved conversions per field.</param>
    /// <typeparam name="TProjection">The <see cref="Type"/> of the projection read model.</typeparam>
    /// <returns>True if resolved, false if not.</returns>
    bool TryResolve<TProjection>(IClientBuildResults buildResults, out IDictionary<string, BsonType> conversions)
        where TProjection : class, new();
}

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
public interface IGetDefaultConversionsFromReadModel
{
    /// <summary>
    /// Try to get the BSON conversions for fields of the <typeparamref name="TReadModel"/> projection read model <see cref="Type"/>.
    /// </summary>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    /// <param name="conversions">The resolved conversions per field.</param>
    /// <typeparam name="TReadModel">The <see cref="Type"/> of the projection read model.</typeparam>
    /// <returns>True if successful, false if not.</returns>
    bool TryGetFrom<TReadModel>(IClientBuildResults buildResults, out IDictionary<string, BsonType> conversions)
        where TReadModel : class, new();
}

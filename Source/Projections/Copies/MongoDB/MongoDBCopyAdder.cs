// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Common.ClientSetup;

namespace Dolittle.SDK.Projections.Copies.MongoDB;

/// <summary>
/// Represents an implementation of <see cref="ICanAugmentProjectionCopy"/> that adds the <see cref="ProjectionCopyToMongoDB"/> definition.
/// </summary>
public class MongoDBCopyAdder : ICanAugmentProjectionCopy
{
    /// <inheritdoc />
    public bool TryAugment<TProjection>(IClientBuildResults buildResults, ProjectionCopies projectionCopies, out ProjectionCopies augmentedProjectionCopies)
        where TProjection : class, new()
        => throw new System.NotImplementedException();
}

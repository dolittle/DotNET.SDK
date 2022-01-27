// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Common.ClientSetup;

namespace Dolittle.SDK.Projections.Copies.MongoDB;

/// <summary>
/// Defines a system that can validate a <see cref="ProjectionMongoDBCopyCollectionName"/>.
/// </summary>
public interface IValidateMongoDBCollectionName
{
    /// <summary>
    /// Validates the <see cref="ProjectionMongoDBCopyCollectionName"/>.
    /// </summary>
    /// <param name="buildResult">The <see cref="IClientBuildResults"/>.</param>
    /// <param name="collectionName">The <see cref="ProjectionMongoDBCopyCollectionName"/>.</param>
    /// <returns>True if the <see cref="ProjectionMongoDBCopyCollectionName"/> is valid, false if not.</returns>
    bool Validate(IClientBuildResults buildResult, ProjectionMongoDBCopyCollectionName collectionName);
}
